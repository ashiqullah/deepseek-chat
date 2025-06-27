namespace DeepSeekChatApi.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? ConversationId { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty; // "user" or "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 