namespace RecruitAI.Application.DTOs;

public class SignupResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
