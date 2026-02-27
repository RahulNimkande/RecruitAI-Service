using RecruitAI.Domain.Entities;

namespace RecruitAI.Application.Interfaces
{
    public interface IRecruiterChatService
    {
        Task<Guid> CreateSessionAsync(Guid recruiterUserId);
        Task SendMessageAsync(Guid recruiterUserId, Guid sessionId, string content);
        Task<List<RecruiterChatMessage>> GetMessagesAsync(Guid recruiterUserId, Guid sessionId);
    }
}
