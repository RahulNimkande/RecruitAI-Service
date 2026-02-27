using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitAI.Api.Extensions;
using RecruitAI.Application.DTOs;
using RecruitAI.Application.Interfaces;

namespace RecruitAI.Api.Controllers
{
    [Authorize(Roles = "Recruiter")]
    [ApiController]
    [Route("api/recruiter/chat")]
    public class RecruiterChatController : ControllerBase
    {
        private readonly IRecruiterChatService _chatService;
        public RecruiterChatController(IRecruiterChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("session")]
        public async Task<IActionResult> CreateSession()
        {
            try
            {
                var userId = User.GetUserId();
                var sessionId = await _chatService.CreateSessionAsync(userId);

                return Ok(sessionId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                await _chatService.SendMessageAsync(userId, request.SessionId, request.Content);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("messages/{sessionId}")]
        public async Task<IActionResult> GetMessages(Guid sessionId)
        {
            try
            {
                var userId = User.GetUserId();
                var messages = await _chatService.GetMessagesAsync(userId, sessionId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
