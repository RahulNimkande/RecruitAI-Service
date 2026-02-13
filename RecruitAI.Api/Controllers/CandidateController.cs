using Microsoft.AspNetCore.Mvc;
using RecruitAI.Application.Services;
using RecruitAI.Application.DTOs;

namespace RecruitAI.Api.Controllers;

[ApiController]
[Route("api/candidate")]
public class CandidateController : ControllerBase
{
    private readonly CandidateService _candidateService;
    private readonly ILogger<CandidateController> _logger;

    public CandidateController(CandidateService candidateService, ILogger<CandidateController> logger)
    {
        _candidateService = candidateService;
        _logger = logger;
    }

    [HttpPost("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateProfileRequest request)
    {
        try
        {
            // TODO: Get UserId from authenticated user
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder

            var candidate = await _candidateService.UpdateCandidateProfileAsync(userId, request.FullName, request.Phone);
            
            var response = new CandidateResponse
            {
                Id = candidate.Id,
                UserId = candidate.UserId,
                FullName = candidate.FullName,
                Phone = candidate.Phone,
                ResumeUrl = candidate.ResumeUrl,
                ProfileJson = candidate.ProfileJson,
                CreatedAt = candidate.CreatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating candidate profile");
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpPost("upload-resume")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty" });

            // TODO: Get UserId from authenticated user
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder

            // Create uploads directory if not exists
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Save file
            var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_{file.FileName}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // TODO: Extract text from resume file (PDF/DOC parsing)
            var resumeText = ""; // Placeholder for extracted text

            // Store resume URL and text
            var candidate = await _candidateService.UploadResumeAsync(userId, filePath, resumeText);

            var response = new CandidateResponse
            {
                Id = candidate.Id,
                UserId = candidate.UserId,
                FullName = candidate.FullName,
                Phone = candidate.Phone,
                ResumeUrl = candidate.ResumeUrl,
                ProfileJson = candidate.ProfileJson,
                CreatedAt = candidate.CreatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading resume");
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            // TODO: Get UserId from authenticated user
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder

            var candidate = await _candidateService.GetCandidateByUserIdAsync(userId);

            var response = new CandidateResponse
            {
                Id = candidate.Id,
                UserId = candidate.UserId,
                FullName = candidate.FullName,
                Phone = candidate.Phone,
                ResumeUrl = candidate.ResumeUrl,
                ProfileJson = candidate.ProfileJson,
                CreatedAt = candidate.CreatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving candidate profile");
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}
