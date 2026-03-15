using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkflowApp.Api.Domain.Entities;
using WorkflowApp.Api.Models.Auth;
using WorkflowApp.Api.Services;
using WorkflowApp.Api.Services.Interfaces;
using WorkflowApp.Api.Tests.Helpers;

namespace WorkflowApp.Api.Tests.Serveices
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ユーザーを作成することを確認する()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();

            // IJwtTokenServiceはRegisterAsyncのテストでは必要ないためモックを作成
            var jwtTokenService = Substitute.For<IJwtTokenService>();

            var authService = new AuthService(dbContext, jwtTokenService);

            var request = new RegisterRequest
            {
                LoginId = "testuser",
                DisplayName = "Test User",
                Password = "Password123"
            };

            // Act
            await authService.RegisterAsync(request, TestContext.Current.CancellationToken);

            // Assert
            var user = await dbContext.User.SingleAsync(x => x.LoginId == "testuser",
                cancellationToken: TestContext.Current.CancellationToken);

            user.DisplayName.Should().Be("Test User");
            user.PasswordHash.Should().NotBeNullOrWhiteSpace();
            user.PasswordHash.Should().NotBe("Password123");
        }


        [Fact]
        public async Task RegisterAsync_既に同じログインIDのユーザーが登録されていたら登録が失敗すること()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();
            var jwtService = Substitute.For<IJwtTokenService>();

            var authService = new AuthService(dbContext, jwtService);

            var request = new RegisterRequest
            {
                LoginId = "testuser",
                DisplayName = "Test User",
                Password = "Password123"
            };

            // 先に1件登録
            await authService.RegisterAsync(request, TestContext.Current.CancellationToken);

            // Act
            async Task action() => await authService.RegisterAsync(request);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        public async Task LoginAsync_有効なパスワードの場合はログインに成功すること()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();

            var jwtService = Substitute.For<IJwtTokenService>();

            // ログイン成功時の期待されるレスポンスを設定
            var expectedResponse = new AuthResponse
            {
                Token = "test-token",
                LoginId = "testuser",
                DisplayName = "Test User",
                Role = "Applicant",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            // IJwtTokenServiceのCreateTokenメソッドが呼び出されたときに、期待されるレスポンスを返すように設定
            jwtService.CreateToken(Arg.Any<User>())
                .Returns(expectedResponse);

            var authService = new AuthService(dbContext, jwtService);

            // 先にユーザーを登録しておく
            var registerRequest = new RegisterRequest
            {
                LoginId = "testuser",
                DisplayName = "Test User",
                Password = "Password123"
            };

            await authService.RegisterAsync(registerRequest, TestContext.Current.CancellationToken);

            // ログインリクエストを作成
            var loginRequest = new LoginRequest
            {
                LoginId = "testuser",
                Password = "Password123"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-token", result.Token);
            Assert.Equal("testuser", result.LoginId);
        }


        [Fact]
        public async Task LoginAsync_無効なパスワードの場合はログインに失敗すること()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();

            var jwtService = Substitute.For<IJwtTokenService>();

            var authService = new AuthService(dbContext, jwtService);

            // 先にユーザーを登録しておく
            var registerRequest = new RegisterRequest
            {
                LoginId = "testuser",
                DisplayName = "Test User",
                Password = "Password123"
            };

            await authService.RegisterAsync(registerRequest, TestContext.Current.CancellationToken);


            // ログインリクエストを作成
            var loginRequest = new LoginRequest
            {
                LoginId = "testuser",
                Password = "WrondPassword"  // 無効なパスワード
            };

            // Act
            var result = await authService.LoginAsync(loginRequest, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);

            // IJwtTokenServiceのCreateTokenメソッドが呼び出されていないことを確認
            jwtService.DidNotReceive().CreateToken(Arg.Any<User>());
        }

        [Fact]
        public async Task LoginAsync_ユーザーが存在しない場合はログインに失敗すること()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();

            var jwtService = Substitute.For<IJwtTokenService>();

            var authService = new AuthService(dbContext, jwtService);

            // ログインリクエストを作成
            var loginRequest = new LoginRequest
            {
                LoginId = "WrongUser",
                Password = "Password123"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);

            // IJwtTokenServiceのCreateTokenメソッドが呼び出されていないことを確認
            jwtService.DidNotReceive().CreateToken(Arg.Any<User>());
        }

        [Fact]
        public async Task LoginAsync_非アクティブなユーザーの場合はログインに失敗すること()
        {
            // Arrange
            var dbContext = TestDbContextFactory.CreateDbContext();

            var jwtService = Substitute.For<IJwtTokenService>();

            var authService = new AuthService(dbContext, jwtService);

            // 先にユーザーを登録しておく
            var registerRequest = new RegisterRequest
            {
                LoginId = "testuser",
                DisplayName = "Test User",
                Password = "Password123"
            };

            await authService.RegisterAsync(registerRequest, TestContext.Current.CancellationToken);

            // 登録したユーザーを非アクティブにする
            var user = await dbContext.User.SingleAsync(x => x.LoginId == "testuser",
                                                        TestContext.Current.CancellationToken);
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            // ログインリクエストを作成
            var loginRequest = new LoginRequest
            {
                LoginId = "testuser",
                Password = "Password123"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);

            // IJwtTokenServiceのCreateTokenメソッドが呼び出されていないことを確認
            jwtService.DidNotReceive().CreateToken(Arg.Any<User>());
        }
    }
}
