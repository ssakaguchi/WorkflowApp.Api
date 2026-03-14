using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkflowApp.Api.Infrastructure.Data;
using WorkflowApp.Api.Models.Auth;
using WorkflowApp.Api.Services;
using WorkflowApp.Api.Services.Interfaces;

namespace WorkflowApp.Api.Tests.Serveices
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsyncがユーザーを作成することを確認する()
        {
            // Arrange
            AppDbContext dbContext = CreateDbContext();

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

        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
