using WorkflowApp.Api.Domain.Entities;
using WorkflowApp.Api.DTO.Auth;

namespace WorkflowApp.Api.Services.Interfaces
{
    public interface IJwtTokenService
    {
        AuthResponse CreateToken(User user);
    }
}
