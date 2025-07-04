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
        private readonly Kernel _apiKernel;
        private readonly Kernel _localKernel;
        private readonly IChatCompletionService _apiChatService;
        private readonly IChatCompletionService _localChatService;
        
        // In-memory storage for chat histories (in production, use a database)
        private static readonly ConcurrentDictionary<string, ChatHistory> _chatHistories = new();
        private static readonly ConcurrentDictionary<string, List<ChatMessage>> _messageHistories = new();

        public DeepSeekChatService(IOptions<DeepSeekSettings> settings, ILogger<DeepSeekChatService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            try
            {
                // Initialize API kernel
                var apiHttpClient = new HttpClient();
                var apiBuilder = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(
                        modelId: _settings.ModelId,
                        endpoint: new Uri(_settings.Endpoint),
                        apiKey: _settings.ApiKey,
                        serviceId: _settings.ServiceId,
                        httpClient: apiHttpClient
                    );

                _apiKernel = apiBuilder.Build();
                _apiChatService = _apiKernel.GetRequiredService<IChatCompletionService>();

                // Initialize Local kernel (Ollama or similar)
                var localHttpClient = new HttpClient();
                var localBuilder = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(
                        modelId: _settings.LocalModelId,
                        endpoint: new Uri(_settings.LocalModelEndpoint),
                        apiKey: "not-required", // Local models typically don't need API keys
                        serviceId: _settings.LocalServiceId,
                        httpClient: localHttpClient
                    );

                _localKernel = localBuilder.Build();
                _localChatService = _localKernel.GetRequiredService<IChatCompletionService>();
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
                var historyKey = $"{request.Provider}_{conversationId}";
                var history = _chatHistories.GetOrAdd(historyKey, _ => new ChatHistory());
                var messageHistory = _messageHistories.GetOrAdd(historyKey, _ => new List<ChatMessage>());

                // Add user message to history
                history.AddUserMessage(request.Message);
                messageHistory.Add(new ChatMessage 
                { 
                    Role = "user", 
                    Content = request.Message,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Processing message for conversation {ConversationId} using {Provider}", 
                    conversationId, request.Provider);

                // Select the appropriate service and kernel based on provider
                IChatCompletionService chatService;
                Kernel kernel;
                
                switch (request.Provider)
                {
                    case ModelProvider.LocalModel:
                        chatService = _localChatService;
                        kernel = _localKernel;
                        break;
                    case ModelProvider.DeepSeekAPI:
                    default:
                        chatService = _apiChatService;
                        kernel = _apiKernel;
                        break;
                }

                // Check if provider is available
                if (!await IsProviderAvailableAsync(request.Provider))
                {
                    throw new InvalidOperationException($"Provider {request.Provider} is not available");
                }

                // Get response from selected service
                var result = await chatService.GetChatMessageContentAsync(history, kernel: kernel);
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
                    Success = true,
                    Provider = request.Provider
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message with provider {Provider}", request.Provider);
                return new ChatResponse
                {
                    Response = $"I'm sorry, an error occurred while processing your message using {request.Provider}. Error: {ex.Message}",
                    ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
                    Success = false,
                    Error = ex.Message,
                    Provider = request.Provider
                };
            }
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string conversationId)
        {
            await Task.CompletedTask; // Placeholder for async operation
            
            // Try to find history with any provider prefix
            var apiKey = $"{ModelProvider.DeepSeekAPI}_{conversationId}";
            var localKey = $"{ModelProvider.LocalModel}_{conversationId}";
            
            if (_messageHistories.TryGetValue(apiKey, out var apiHistory))
                return new List<ChatMessage>(apiHistory);
            
            if (_messageHistories.TryGetValue(localKey, out var localHistory))
                return new List<ChatMessage>(localHistory);
            
            return new List<ChatMessage>();
        }

        public async Task ClearChatHistoryAsync(string conversationId)
        {
            await Task.CompletedTask; // Placeholder for async operation
            
            // Clear history for all providers
            var apiKey = $"{ModelProvider.DeepSeekAPI}_{conversationId}";
            var localKey = $"{ModelProvider.LocalModel}_{conversationId}";
            
            _chatHistories.TryRemove(apiKey, out _);
            _messageHistories.TryRemove(apiKey, out _);
            _chatHistories.TryRemove(localKey, out _);
            _messageHistories.TryRemove(localKey, out _);
        }

        public async Task<bool> IsProviderAvailableAsync(ModelProvider provider)
        {
            try
            {
                switch (provider)
                {
                    case ModelProvider.DeepSeekAPI:
                        _logger.LogInformation("Checking DeepSeek API availability at {Endpoint}", _settings.Endpoint);
                        
                        // Check if API is accessible with authentication
                        using (var client = new HttpClient())
                        {
                            client.Timeout = TimeSpan.FromSeconds(10);
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
                            
                            // Try the models endpoint with auth, or fall back to a simple connectivity check
                            try
                            {
                                var modelsUrl = _settings.Endpoint.TrimEnd('/') + "/models";
                                _logger.LogInformation("Testing DeepSeek API models endpoint: {Url}", modelsUrl);
                                var response = await client.GetAsync(modelsUrl);
                                _logger.LogInformation("DeepSeek API models endpoint response: {StatusCode}", response.StatusCode);
                                return response.IsSuccessStatusCode;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "DeepSeek API models endpoint failed, falling back to API key validation");
                                // If models endpoint fails, just check if we can reach the base endpoint
                                // This is a more lenient check - if API key is valid, we assume it's available
                                var isApiKeyValid = !string.IsNullOrEmpty(_settings.ApiKey) && 
                                                   _settings.ApiKey != "your-api-key-here" && 
                                                   _settings.ApiKey.StartsWith("sk-");
                                _logger.LogInformation("DeepSeek API key validation result: {IsValid}", isApiKeyValid);
                                return isApiKeyValid;
                            }
                        }
                    
                    case ModelProvider.LocalModel:
                        _logger.LogInformation("Checking Local Model availability at {Endpoint}", _settings.LocalModelEndpoint);
                        
                        // Check if local model endpoint is accessible
                        using (var client = new HttpClient())
                        {
                            client.Timeout = TimeSpan.FromSeconds(5);
                            
                            // Try multiple endpoints to check Ollama availability
                            try
                            {
                                // First try the version endpoint (simpler check)
                                var versionUrl = _settings.LocalModelEndpoint.TrimEnd('/').Replace("/v1", "") + "/api/version";
                                _logger.LogInformation("Testing Ollama version endpoint: {Url}", versionUrl);
                                var versionResponse = await client.GetAsync(versionUrl);
                                _logger.LogInformation("Ollama version endpoint response: {StatusCode}", versionResponse.StatusCode);
                                if (versionResponse.IsSuccessStatusCode)
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Ollama version endpoint failed");
                            }
                            
                            try
                            {
                                // Try the models endpoint
                                var modelsUrl = _settings.LocalModelEndpoint.TrimEnd('/') + "/models";
                                _logger.LogInformation("Testing Ollama models endpoint: {Url}", modelsUrl);
                                var modelsResponse = await client.GetAsync(modelsUrl);
                                _logger.LogInformation("Ollama models endpoint response: {StatusCode}", modelsResponse.StatusCode);
                                return modelsResponse.IsSuccessStatusCode;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Ollama models endpoint failed");
                            }
                            
                            try
                            {
                                // Try basic connectivity check to Ollama
                                var baseUrl = _settings.LocalModelEndpoint.TrimEnd('/').Replace("/v1", "");
                                var tagsUrl = $"{baseUrl}/api/tags";
                                _logger.LogInformation("Testing Ollama tags endpoint: {Url}", tagsUrl);
                                var response = await client.GetAsync(tagsUrl);
                                _logger.LogInformation("Ollama tags endpoint response: {StatusCode}", response.StatusCode);
                                return response.IsSuccessStatusCode;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Ollama tags endpoint failed");
                            }
                            
                            _logger.LogWarning("All Ollama endpoint checks failed");
                            return false;
                        }
                    
                    default:
                        _logger.LogWarning("Unknown provider: {Provider}", provider);
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {Provider} availability check failed with exception", provider);
                return false;
            }
        }
    }
} 