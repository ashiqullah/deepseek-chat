namespace DeepSeekChatApi.Models
{
    public class DeepSeekSettings
    {
        public string ModelId { get; set; } = "deepseek-chat";
        public string ApiKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = "https://api.deepseek.com/v1/";
        public string ServiceId { get; set; } = "deepseek";
        
        // Local model settings
        public string LocalModelEndpoint { get; set; } = "http://localhost:11434/v1/"; // Default Ollama endpoint
        public string LocalModelId { get; set; } = "deepseek-r1:latest";
        public string LocalServiceId { get; set; } = "local-deepseek";
    }
} 