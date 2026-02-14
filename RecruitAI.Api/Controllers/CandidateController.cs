using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecruitAI.Application.DTOs;
using RecruitAI.Application.Services;
using System.Security.Claims;

namespace RecruitAI.Api.Controllers;

[Authorize ]
[ApiController]
[Route("api/candidate")]
public class CandidateController : ControllerBase
{
    private readonly CandidateService _candidateService;
    private readonly ILogger<CandidateController> _logger;
    private readonly StorageSettings _storageSettings;

    public CandidateController(CandidateService candidateService, ILogger<CandidateController> logger, IOptions<StorageSettings> storageOptions)
    {
        _candidateService = candidateService;
        _logger = logger;
        _storageSettings = storageOptions.Value;
    }

    [HttpPost("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateProfileRequest request)
    {
        try
        {
            var userId = Guid.Parse(input: User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Placeholder

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

            var userId = Guid.Parse(input: User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Create uploads directory if not exists
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), _storageSettings.UploadPath);
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
            //var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

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
