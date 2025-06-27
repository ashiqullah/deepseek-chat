using Microsoft.AspNetCore.Mvc;
using DeepSeekChatApi.Models;
using DeepSeekChatApi.Services;

namespace DeepSeekChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IDeepSeekChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IDeepSeekChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest("Message cannot be empty");
                }

                var response = await _chatService.SendMessageAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessage endpoint");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("history/{conversationId}")]
        public async Task<ActionResult<List<ChatMessage>>> GetChatHistory(string conversationId)
        {
            try
            {
                var history = await _chatService.GetChatHistoryAsync(conversationId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetChatHistory endpoint");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("history/{conversationId}")]
        public async Task<ActionResult> ClearChatHistory(string conversationId)
        {
            try
            {
                await _chatService.ClearChatHistoryAsync(conversationId);
                return Ok(new { message = "Chat history cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearChatHistory endpoint");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public ActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
} 