using Microsoft.EntityFrameworkCore;
using RecruitAI.Application.Interfaces;
using RecruitAI.Domain.Entities;
using RecruitAI.Infrastructure.Persistence;

namespace RecruitAI.Application.Services
{
    public class RecruiterChatService : IRecruiterChatService
    {
        private readonly RecruitAIDbContext _context;
        public RecruiterChatService(RecruitAIDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateSessionAsync(Guid recruiterUserId)
        {
            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == recruiterUserId);

            if (recruiter == null)
                throw new InvalidOperationException("Recruiter profile not found");

            var session = new RecruiterChatSession
            {
                RecruiterId = recruiter.Id
            };

            _context.RecruiterChatSessions.Add(session);
            await _context.SaveChangesAsync();

            return session.Id;
        }

        public async Task SendMessageAsync(Guid recruiterUserId, Guid sessionId, string content)
        {
            var session = await _context.RecruiterChatSessions
                .Include(s => s.Recruiter)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new InvalidOperationException("Chat session not found");

            if (session.Recruiter.UserId != recruiterUserId)
                throw new UnauthorizedAccessException("You do not have access to this chat session");

            var message = new RecruiterChatMessage
            {
                SessionId = sessionId,
                IsFromRecruiter = true,
                Content = content
            };

            _context.RecruiterChatMessages.Add(message);
            await _context.SaveChangesAsync();

        }
        public async Task<List<RecruiterChatMessage>> GetMessagesAsync(Guid recruiterUserId, Guid sessionId)
        {
            var session = await _context.RecruiterChatSessions
                .Include(s => s.Recruiter)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new InvalidOperationException("Chat session not found");

            if (session.Recruiter.UserId != recruiterUserId)
                throw new UnauthorizedAccessException("You do not have access to this chat session");

            return await _context.RecruiterChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
