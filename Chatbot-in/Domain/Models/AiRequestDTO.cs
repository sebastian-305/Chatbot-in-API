namespace Chatbot_in.Domain.Models
{
    public class AiRequestDTO
    {
        public string Prompt { get; set; }
        public string[] FileNames { get; set; }
    }
}
