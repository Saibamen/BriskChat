using Microsoft.AspNetCore.SignalR;

namespace TrollChat.Web.Hubs
{
    public class ChannelHub : Hub
    {
        public void Send(string userName, string message)
        {
            Clients.All.broadcastMessage(userName, message);
        }
    }
}