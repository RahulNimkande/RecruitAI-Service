using System.ComponentModel.DataAnnotations;

namespace RecruitAI.Application.DTOs;

public class UpdateCandidateProfileRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = null!;

    [Required]
    [Phone]
    [StringLength(30, MinimumLength = 7)]
    public string Phone { get; set; } = null!;
}
