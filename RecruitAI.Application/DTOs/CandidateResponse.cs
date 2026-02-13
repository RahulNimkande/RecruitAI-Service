namespace RecruitAI.Application.DTOs;

public class CandidateResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string ResumeUrl { get; set; }
    public string ProfileJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
