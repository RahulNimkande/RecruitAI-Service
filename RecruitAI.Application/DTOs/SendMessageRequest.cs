using System.ComponentModel.DataAnnotations;

namespace RecruitAI.Application.DTOs
{
    public class SendMessageRequest
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Content { get; set; } = null!;
    }

}
