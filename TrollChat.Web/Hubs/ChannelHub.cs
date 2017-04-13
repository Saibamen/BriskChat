using System;
using Microsoft.AspNetCore.SignalR;

namespace TrollChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        [Authorize(Roles = "Admin")]
        public void Send(string userName, string message)
        {
            if (string.IsNullOrEmpty(userName.Trim()) || string.IsNullOrEmpty(message.Trim()))
            {
                return;
            }

            // TODO: Save to database

            DateTime timestamp = DateTime.Now;
            var chatTime = timestamp.ToString("HH:mm");

            Clients.All.broadcastMessage(userName.Trim(), message.Trim(), chatTime);
        }
    }
}