using WorkflowApp.Api.Models.Auth;

namespace WorkflowApp.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    }
}
