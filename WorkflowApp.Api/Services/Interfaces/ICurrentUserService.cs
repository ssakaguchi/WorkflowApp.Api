using System.Security.Claims;
using WorkflowApp.Api.DTOs.Auth;

namespace WorkflowApp.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        MeResponse? GetCurrentUser(ClaimsPrincipal user);
    }
}
