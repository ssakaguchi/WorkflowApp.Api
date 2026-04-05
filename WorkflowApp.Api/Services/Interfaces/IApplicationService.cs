using WorkflowApp.Api.DTOs.Applications;

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
