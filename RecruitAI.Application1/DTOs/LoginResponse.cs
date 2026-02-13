namespace RecruitAI.Application.DTOs;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}
