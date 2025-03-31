namespace Chatbot_in.Domain.Models;

public class BlackListTokenDTO
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }    
}
