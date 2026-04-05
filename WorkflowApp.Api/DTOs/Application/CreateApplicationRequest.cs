using System.ComponentModel.DataAnnotations;

namespace WorkflowApp.Api.DTOs.Application
{
    /// <summary>
    /// ワークフロー申請の新規作成リクエスト
    /// </summary>
    public class CreateApplicationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
