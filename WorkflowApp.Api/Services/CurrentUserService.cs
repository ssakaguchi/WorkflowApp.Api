using System.Security.Claims;
using WorkflowApp.Api.DTOs.Auth;
using WorkflowApp.Api.Services.Interfaces;

namespace WorkflowApp.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public MeResponse? GetCurrentUser(ClaimsPrincipal user)
        {
            var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loginId = user.FindFirst(ClaimTypes.Name)?.Value;
            var displayName = user.FindFirst("displayName")?.Value;

            if (string.IsNullOrWhiteSpace(userIdValue) ||
                string.IsNullOrWhiteSpace(loginId) ||
                !int.TryParse(userIdValue, out var userId))
            {
                return null;
            }

            return new MeResponse
            {
                UserId = userId,
                LoginId = loginId,
                DisplayName = displayName ?? string.Empty
            };
        }
    }
}
