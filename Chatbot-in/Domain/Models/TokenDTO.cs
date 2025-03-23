namespace Chatbot_in.Domain.Models
{
    public class TokenDTO
    {
        public string token { get; internal set; }
        public string id { get; internal set; }
        public DateTime expiration { get; internal set; }
        public IList<string> roles { get; internal set; }
    }
}
