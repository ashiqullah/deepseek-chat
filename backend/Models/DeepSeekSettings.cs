namespace DeepSeekChatApi.Models
{
    public class DeepSeekSettings
    {
        public string ModelId { get; set; } = "deepseek-chat";
        public string ApiKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = "https://api.deepseek.com/v1/";
        public string ServiceId { get; set; } = "deepseek";
    }
} 