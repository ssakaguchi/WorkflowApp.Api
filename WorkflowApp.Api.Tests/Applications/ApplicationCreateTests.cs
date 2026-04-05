using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WorkflowApp.Api.Domain.Entities;
using WorkflowApp.Api.DTOs.Applications;
using WorkflowApp.Api.Infrastructure.Data;
using WorkflowApp.Api.Services.Interfaces;
using WorkflowApp.Api.Tests.Helpers;

namespace WorkflowApp.Api.Tests.Applications
{
    public class ApplicationCreateTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ApplicationCreateTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task Post_認証済みユーザーが有効な申請を送信した場合_201Createdを返しDBに保存されること()
        {
            // Arrange
            var client = _factory.CreateClient();

            int userId;
            string token;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

                // テストユーザーの作成
                var user = new User
                {
                    LoginId = "applicant01",
                    DisplayName = "テスト申請者",
                    PasswordHash = "dummy-hash",
                    Role = "Applicant",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

                userId = user.Id;

                token = jwtTokenService.CreateToken(user).Token;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                Title = "テスト申請",
                Content = "これはテストの申請です。",
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/applications",
                                                        request,
                                                        cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>(cancellationToken: TestContext.Current.CancellationToken);

            responseBody.Should().NotBeNull();
            responseBody!.Title.Should().Be(request.Title);
            responseBody.Content.Should().Be(request.Content);

            using var verfyScope = _factory.Services.CreateScope();
            var verifyDbContext = verfyScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var savedApplication = verifyDbContext.Applications.Single();
            savedApplication.Title.Should().Be(request.Title);
            savedApplication.Content.Should().Be(request.Content);
            savedApplication.ApplicantUserId.Should().Be(userId);   
            savedApplication.Status.Should().Be("Pending");
        }
    }
}
