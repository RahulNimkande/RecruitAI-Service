using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitAI.Domain.Entities
{
    public class RecruiterChatSession
    {
        public Guid Id { get; set; }

        public Guid RecruiterId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ExtractedRequirementJson { get; set; }

        public Recruiter Recruiter { get; set; } = null!;

        public ICollection<RecruiterChatMessage> Messages { get; set; }
            = new List<RecruiterChatMessage>();
    }

}
