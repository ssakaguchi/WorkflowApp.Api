using Microsoft.AspNetCore.Mvc;
using WorkflowApp.Api.Models.Auth;
using WorkflowApp.Api.Services.Interfaces;

namespace WorkflowApp.Api.Controllers;

/// <summary>
/// ユーザー認証および登録に関するAPIエンドポイントを提供します
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// ユーザーを新規登録します。
    /// </summary>
    /// <param name="request">登録情報（null不可・バリデーション必須）。</param>
    /// <param name="cancellationToken">処理キャンセル用トークン。</param>
    /// <returns>
    /// 登録結果を返します（成功: 200 OK、既存ユーザー: 409 Conflict、入力不正: バリデーションエラー）
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _authService.RegisterAsync(request, cancellationToken);
            return Ok(new { message = "ユーザーを登録しました。" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// ログイン情報をもとにユーザーを認証します
    /// </summary>
    /// <param name="request">認証対象のログイン情報（null不可）</param>
    /// <param name="cancellationToken">処理キャンセル用トークン</param>
    /// <returns>
    /// 認証結果を返します（成功: 200 OK、失敗: 401 Unauthorized、入力不正: バリデーションエラー）
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new { message = "ログインIDまたはパスワードが正しくありません。" });
        }

        return Ok(response);
    }
}