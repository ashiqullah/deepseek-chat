using Microsoft.AspNetCore.Mvc;
using DeepSeekChatApi.Models;
using DeepSeekChatApi.Services;
using Microsoft.Extensions.Options;

namespace DeepSeekChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IDeepSeekChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        private readonly DeepSeekSettings _settings;

        public ChatController(IDeepSeekChatService chatService, ILogger<ChatController> logger, IOptions<DeepSeekSettings> settings)
        {
            _chatService = chatService;
            _logger = logger;
            _settings = settings.Value;
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

        [HttpGet("providers")]
        public async Task<ActionResult> GetProviderStatus()
        {
            try
            {
                var apiAvailable = await _chatService.IsProviderAvailableAsync(ModelProvider.DeepSeekAPI);
                var localAvailable = await _chatService.IsProviderAvailableAsync(ModelProvider.LocalModel);

                return Ok(new
                {
                    providers = new[]
                    {
                        new { name = "DeepSeek API", value = ModelProvider.DeepSeekAPI, available = apiAvailable },
                        new { name = "Local Model", value = ModelProvider.LocalModel, available = localAvailable }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProviderStatus endpoint");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("providers/debug")]
        public async Task<ActionResult> GetProviderDebugInfo()
        {
            try
            {
                var debugInfo = new
                {
                    configuration = new
                    {
                        deepSeekEndpoint = _settings.Endpoint,
                        deepSeekApiKeySet = !string.IsNullOrEmpty(_settings.ApiKey),
                        deepSeekApiKeyLength = _settings.ApiKey?.Length ?? 0,
                        deepSeekApiKeyPrefix = _settings.ApiKey?.Substring(0, Math.Min(7, _settings.ApiKey?.Length ?? 0)),
                        localModelEndpoint = _settings.LocalModelEndpoint,
                        localModelId = _settings.LocalModelId
                    },
                    checks = new
                    {
                        deepSeekApiAvailable = await _chatService.IsProviderAvailableAsync(ModelProvider.DeepSeekAPI),
                        localModelAvailable = await _chatService.IsProviderAvailableAsync(ModelProvider.LocalModel)
                    },
                    connectivity = await CheckConnectivity()
                };

                return Ok(debugInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProviderDebugInfo endpoint");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<object> CheckConnectivity()
        {
            var results = new Dictionary<string, object>();

            // Test DeepSeek API connectivity
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync("https://api.deepseek.com/");
                results["deepSeekBaseReachable"] = response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                results["deepSeekBaseReachable"] = $"Failed: {ex.Message}";
            }

            // Test local model connectivity
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                var response = await client.GetAsync(_settings.LocalModelEndpoint.Replace("/v1", ""));
                results["localModelReachable"] = response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                results["localModelReachable"] = $"Failed: {ex.Message}";
            }

            return results;
        }

        [HttpGet("health")]
        public ActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
} 