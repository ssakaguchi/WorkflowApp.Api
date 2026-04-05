using Microsoft.EntityFrameworkCore;
using WorkflowApp.Api.DTOs.Application;
using WorkflowApp.Api.Infrastructure.Data;
using WorkflowApp.Api.Services;

namespace WorkflowApp.Api.Tests.Serveices
{
    public class ApplicationServiceTests
    {
        [Fact]
        public async Task CreateAsync_正常なリクエストの場合は申請を保存してIdを返すこと()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new AppDbContext(options);
            var service = new ApplicationService(dbContext);

            var request = new CreateApplicationRequest
            {
                Title = "出張申請",
                Content = "4月10日の東京出張について申請します。"
            };

            var userId = 1;

            // Act
            var applicationId = await service.CreateAsync(request, userId, CancellationToken.None);

            // Assert
            var savedApplication = await dbContext.Applications.SingleAsync(CancellationToken.None);

            Assert.True(applicationId > 0);

            Assert.Equal(request.Title, savedApplication.Title);
            Assert.Equal(request.Content, savedApplication.Content);
            Assert.Equal("Pending", savedApplication.Status);
            Assert.Equal(userId, savedApplication.ApplicantUserId);

            // CreatedAtは現在時刻とほぼ同じであることを確認
            Assert.True(savedApplication.CreatedAt <= DateTime.UtcNow);

            // CreatedAtが過去1分以内であることを確認
            Assert.True(savedApplication.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        }
    }
}
