using Microsoft.AspNetCore.SignalR;

namespace TrollChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        [Authorize(Roles = "Admin")]
        public void Send(string userName, string message)
        {
            Clients.All.broadcastMessage(userName, message);
        }
    }
}