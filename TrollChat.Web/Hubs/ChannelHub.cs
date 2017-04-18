using System;
using Microsoft.AspNetCore.SignalR;
using TrollChat.BusinessLogic.Actions.User.Interfaces;

namespace TrollChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        private readonly IGetUserById getUserById;

        public ChannelHub(IGetUserById getUserById)
        {
            this.getUserById = getUserById;
        }

        public void Send(/*string userName, */string message)
        {
            if (/*string.IsNullOrEmpty(userName.Trim()) || */string.IsNullOrEmpty(message.Trim()))
            {
                return;
            }

            int fakeUserId = 1;
            DateTime timestamp = DateTime.Now;
            // TODO: Only timestamp
            var chatTime = timestamp.ToString("HH:mm");

            var user = getUserById.Invoke(fakeUserId);

            if (user != null)
            {
                // TODO: Save to database

                Clients.All.broadcastMessage(user.Name.Trim(), message.Trim(), chatTime);
            }
        }
    }
}