using WorkflowApp.Api.DTOs.Application;

namespace WorkflowApp.Api.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<int> CreateAsync(
            CreateApplicationRequest request,
            int userId,
            CancellationToken cancellationToken);
    }
}
