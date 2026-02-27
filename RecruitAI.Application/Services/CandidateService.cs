using Microsoft.EntityFrameworkCore;
using RecruitAI.Domain.Entities;
using RecruitAI.Infrastructure.Persistence;

namespace RecruitAI.Application.Services;

public class CandidateService
{
    private readonly RecruitAIDbContext _context;

    public CandidateService(RecruitAIDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate> UpdateCandidateProfileAsync(Guid userId, string fullName, string phone)
    {
        var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate profile not found");

        candidate.FullName = fullName;
        candidate.Phone = phone;

        _context.Candidates.Update(candidate);
        await _context.SaveChangesAsync();

        return candidate;
    }

    public async Task<Candidate> UploadResumeAsync(Guid userId, string resumeUrl, string resumeText)
    {
        var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate profile not found");

        candidate.ResumeUrl = resumeUrl;
        candidate.ResumeText = resumeText;

        _context.Candidates.Update(candidate);
        await _context.SaveChangesAsync();

        return candidate;
    }

    public async Task<Candidate> UpdateProfileJsonAsync(Guid userId, string profileJson)
    {
        var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate profile not found");

        candidate.ProfileJson = profileJson;

        _context.Candidates.Update(candidate);
        await _context.SaveChangesAsync();

        return candidate;
    }

    public async Task<Candidate> GetCandidateByUserIdAsync(Guid userId)
    {
        var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate profile not found");

        return candidate;
    }
}
