using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Options;
using DeepSeekChatApi.Models;
using System.Collections.Concurrent;

namespace DeepSeekChatApi.Services
{
    public class DeepSeekChatService : IDeepSeekChatService
    {
        private readonly DeepSeekSettings _settings;
        private readonly ILogger<DeepSeekChatService> _logger;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        
        // In-memory storage for chat histories (in production, use a database)
        private static readonly ConcurrentDictionary<string, ChatHistory> _chatHistories = new();
        private static readonly ConcurrentDictionary<string, List<ChatMessage>> _messageHistories = new();

        public DeepSeekChatService(IOptions<DeepSeekSettings> settings, ILogger<DeepSeekChatService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            try
            {
                var httpClient = new HttpClient();

                var builder = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(
                        modelId: _settings.ModelId,
                        endpoint: new Uri(_settings.Endpoint),
                        apiKey: _settings.ApiKey,
                        serviceId: _settings.ServiceId,
                        httpClient: httpClient
                    );

                _kernel = builder.Build();
                _chatService = _kernel.GetRequiredService<IChatCompletionService>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize DeepSeek chat service");
                throw;
            }
        }

        public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
        {
            try
            {
                var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();
                
                // Get or create chat history
                var history = _chatHistories.GetOrAdd(conversationId, _ => new ChatHistory());
                var messageHistory = _messageHistories.GetOrAdd(conversationId, _ => new List<ChatMessage>());

                // Add user message to history
                history.AddUserMessage(request.Message);
                messageHistory.Add(new ChatMessage 
                { 
                    Role = "user", 
                    Content = request.Message,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Processing message for conversation {ConversationId}", conversationId);

                // Get response from DeepSeek
                var result = await _chatService.GetChatMessageContentAsync(history, kernel: _kernel);
                var responseContent = result.Content ?? "No response received";

                // Add assistant message to history
                history.AddAssistantMessage(responseContent);
                messageHistory.Add(new ChatMessage 
                { 
                    Role = "assistant", 
                    Content = responseContent,
                    Timestamp = DateTime.UtcNow
                });

                return new ChatResponse
                {
                    Response = responseContent,
                    ConversationId = conversationId,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return new ChatResponse
                {
                    Response = "I'm sorry, an error occurred while processing your message.",
                    ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string conversationId)
        {
            await Task.CompletedTask; // Placeholder for async operation
            return _messageHistories.TryGetValue(conversationId, out var history) 
                ? new List<ChatMessage>(history) 
                : new List<ChatMessage>();
        }

        public async Task ClearChatHistoryAsync(string conversationId)
        {
            await Task.CompletedTask; // Placeholder for async operation
            _chatHistories.TryRemove(conversationId, out _);
            _messageHistories.TryRemove(conversationId, out _);
        }
    }
} 