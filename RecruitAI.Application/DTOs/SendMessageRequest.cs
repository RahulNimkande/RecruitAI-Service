using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitAI.Application.DTOs
{
    public class SendMessageRequest
    {
        public Guid SessionId { get; set; }
        public string Content { get; set; } = null!;
    }

}

