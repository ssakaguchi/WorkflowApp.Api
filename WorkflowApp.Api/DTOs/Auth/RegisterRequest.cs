using System.ComponentModel.DataAnnotations;

namespace WorkflowApp.Api.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string LoginId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
