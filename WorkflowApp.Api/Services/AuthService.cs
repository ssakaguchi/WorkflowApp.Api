using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkflowApp.Api.Domain.Entities;
using WorkflowApp.Api.Infrastructure.Data;
using WorkflowApp.Api.Models.Auth;
using WorkflowApp.Api.Services.Interfaces;

namespace WorkflowApp.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(AppDbContext dbContext, IJwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// 非同期操作として、新しいユーザーを登録します。
        /// </summary>
        /// <param name="request">登録するユーザーの情報を含むリクエスト</param>
        /// <param name="cancellationToken">操作のキャンセルを通知するためのトークン</param>
        /// <returns>登録処理の非同期操作を表すタスク。</returns>
        /// <exception cref="InvalidOperationException">同じLoginIdのユーザーが既に存在する場合にスローされます。</exception>
        public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext.User
            .AnyAsync(x => x.LoginId == request.LoginId, cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("同じLoginIdのユーザーが既に存在します。");
            }

            var user = new User
            {
                LoginId = request.LoginId,
                DisplayName = request.DisplayName,
                Role = "Applicant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _dbContext.User.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
<<<<<<< HEAD

        public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // ユーザーの取得
            var user = await _dbContext.User
                .FirstOrDefaultAsync(
                    x => x.LoginId == request.LoginId && x.IsActive,
                    cancellationToken);

            if (user is null)
            {
                return null;
            }

            // パスワードの検証
            var verifyResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // パスワードのハッシュアルゴリズムが古い場合は再ハッシュして保存
            if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return _jwtTokenService.CreateToken(user);
        }
=======
>>>>>>> 2e5140c17adbf9afcb085007e0d473288c419986
    }
}
