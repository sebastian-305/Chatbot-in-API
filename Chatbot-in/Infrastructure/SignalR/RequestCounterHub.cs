using System.Text.Encodings.Web;
using Microsoft.AspNetCore.SignalR;
using static Chatbot_in.Infrastructure.SignalR.ISubscribeCounterClient;

namespace Chatbot_in.Infrastructure.SignalR;

public class RequestCounterHub : Hub<ISubscribeCounterClient>
{
    public async Task Send(RequestsLeftParams message)
    {
        //var encodedMessage = HtmlEncoder.Default.Encode(message);
        await Clients.All.RequestCounterChanged(message);
    }

}
