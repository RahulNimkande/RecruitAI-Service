namespace RecruitAI.Application.DTOs;

public class SignupRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "Candidate" or "Recruiter"
}
