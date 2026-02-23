using RecruitAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitAI.Application.Interfaces
{
    public interface IRecruiterChatService
    {
        Task<Guid> CreateSessionAsync(Guid recruiterId);
        Task SendMessageAsync(Guid sessionId, string content);
        Task<List<RecruiterChatMessage>> GetMessagesAsync(Guid sessionId);
    }
}
