using DeepSeekChatApi.Models;

namespace DeepSeekChatApi.Services
{
    public interface IDeepSeekChatService
    {
        Task<ChatResponse> SendMessageAsync(ChatRequest request);
        Task<List<ChatMessage>> GetChatHistoryAsync(string conversationId);
        Task ClearChatHistoryAsync(string conversationId);
        Task<bool> IsProviderAvailableAsync(ModelProvider provider);
    }
} 