using WorkflowApp.Api.DTO.Auth;

namespace WorkflowApp.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

        Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
