using System.ComponentModel.DataAnnotations;

namespace RecruitAI.Application.DTOs;

public class SignupRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    [Required]
    [RegularExpression("^(Candidate|Recruiter)$", ErrorMessage = "Role must be 'Candidate' or 'Recruiter'.")]
    public string Role { get; set; } = null!; // "Candidate" or "Recruiter"
}
