namespace Chatbot_in.Domain.Service
{
    public interface IAnalyseTemp
    {
        Task<object> SendMessage(string Message);

        Task<object> SendContext(IEnumerable<string> files);
    }
}
