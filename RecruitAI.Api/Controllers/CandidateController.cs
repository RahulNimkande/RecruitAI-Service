using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecruitAI.Api.Extensions;
using RecruitAI.Application.DTOs;
using RecruitAI.Application.Services;
using System.Text.RegularExpressions;

namespace RecruitAI.Api.Controllers;

[Authorize(Roles = "Candidate")]
[ApiController]
[Route("api/candidate")]
public class CandidateController : ControllerBase
{
    private const long MaxResumeSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx"];

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
            var userId = User.GetUserId();
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
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

            if (file.Length > MaxResumeSizeBytes)
                return BadRequest(new { message = "File exceeds 5MB limit" });

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Unsupported file type. Allowed types: .pdf, .doc, .docx" });

            var userId = User.GetUserId();
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), _storageSettings.UploadPath);
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var originalName = Path.GetFileNameWithoutExtension(file.FileName);
            var safeName = Regex.Replace(originalName, "[^a-zA-Z0-9_-]", "_");
            var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_{safeName}{fileExtension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // TODO: Extract text from resume file (PDF/DOC parsing)
            var resumeText = string.Empty;
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
            var userId = User.GetUserId();
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
