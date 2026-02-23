using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitAI.Domain.Entities
{
    public class RecruiterChatMessage
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public bool IsFromRecruiter { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RecruiterChatSession Session { get; set; } = null!;
    }

}
