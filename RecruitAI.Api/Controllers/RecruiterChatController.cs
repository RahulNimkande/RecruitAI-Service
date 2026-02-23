using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitAI.Application.DTOs;
using RecruitAI.Application.Interfaces;
using System.Security.Claims;

namespace RecruitAI.Api.Controllers
{
    [Authorize]
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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var recruiterId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sessionId = await _chatService.CreateSessionAsync(userId);

            return Ok(sessionId);
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            await _chatService.SendMessageAsync(request.SessionId, request.Content);
            return Ok();
        }

        [HttpGet("messages/{sessionId}")]
        public async Task<IActionResult> GetMessages(Guid sessionId)
        {
            var messages = await _chatService.GetMessagesAsync(sessionId);
            return Ok(messages);
        }
    }
}
