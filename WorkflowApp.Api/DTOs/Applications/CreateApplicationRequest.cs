using System.ComponentModel.DataAnnotations;

namespace WorkflowApp.Api.DTOs.Applications
{
    /// <summary>
    /// ワークフロー申請の新規作成リクエスト
    /// </summary>
    public class CreateApplicationRequest
    {
        [Required(ErrorMessage = "件名は必須です。")]
        [MaxLength(100, ErrorMessage = "件名は100文字以内で入力してください。")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "申請内容は必須です。")]
        [MaxLength(1000, ErrorMessage = "申請内容は1000文字以内で入力してください。")]
        public string Content { get; set; } = string.Empty;
    }
}
