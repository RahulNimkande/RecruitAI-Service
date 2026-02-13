using RecruitAI.Domain.Entities;

public class Candidate
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? ResumeUrl { get; set; }

    public string? ResumeText { get; set; }

    public string? ProfileJson { get; set; }

    public DateTime CreatedAt { get; set; }
}
