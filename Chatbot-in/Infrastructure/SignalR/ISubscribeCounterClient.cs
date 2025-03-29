namespace Chatbot_in.Infrastructure.SignalR;

public interface ISubscribeCounterClient
{
    public class RequestsLeftParams
    {
        public required int PromptsLeftMinute { get; set; }
        public required int PromptsLeftDay { get; set; }
    }

    Task RequestCounterChanged(RequestsLeftParams parameters);
}
