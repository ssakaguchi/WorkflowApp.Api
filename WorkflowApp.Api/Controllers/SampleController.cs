using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkflowApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                LoginId = User.Identity?.Name,
                Role = User.FindFirstValue(ClaimTypes.Role),
                DisplayName = User.FindFirst("displayName")?.Value
            });
        }
    }
}
