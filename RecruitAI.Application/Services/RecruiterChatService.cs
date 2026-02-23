using Microsoft.EntityFrameworkCore;
using RecruitAI.Application.Interfaces;
using RecruitAI.Domain.Entities;
using RecruitAI.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitAI.Application.Services
{
    public class RecruiterChatService : IRecruiterChatService
    {
        private readonly RecruitAIDbContext _context;
        public RecruiterChatService(RecruitAIDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateSessionAsync(Guid userId)
        {
            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);

            if (recruiter == null)
                throw new Exception("Recruiter profile not found");

            var session = new RecruiterChatSession
            {
                RecruiterId = recruiter.Id
            };

            _context.RecruiterChatSessions.Add(session);
            await _context.SaveChangesAsync();

            return session.Id;
        }

        public async Task SendMessageAsync(Guid sessionId, string content)
        {
            var message = new RecruiterChatMessage
            {
                SessionId = sessionId,
                IsFromRecruiter = true,
                Content = content
            };

            _context.RecruiterChatMessages.Add(message);
            await _context.SaveChangesAsync();

        }
        public async Task<List<RecruiterChatMessage>> GetMessagesAsync(Guid sessionId)
        {
            return await _context.RecruiterChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
