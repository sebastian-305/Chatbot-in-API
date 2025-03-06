namespace Chatbot_in.Domain.Models
{
    public class Token
    {
        public string TokenId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUse { get; set; }

    }
}
