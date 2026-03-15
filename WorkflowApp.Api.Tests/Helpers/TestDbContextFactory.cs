using Microsoft.EntityFrameworkCore;
using WorkflowApp.Api.Infrastructure.Data;

namespace WorkflowApp.Api.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        /// <summary>
        /// インメモリデータベースを使用してAppDbContextのインスタンスを作成する
        /// </summary>
        /// <returns></returns>
        public static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
