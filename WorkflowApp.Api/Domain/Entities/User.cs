namespace WorkflowApp.Api.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        public string LoginId { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Applicant";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
