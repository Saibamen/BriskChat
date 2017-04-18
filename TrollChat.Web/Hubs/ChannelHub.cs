using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public async Task JoinRoom(string roomId)
        {
            await Groups.Add(Context.ConnectionId, roomId);

            DateTime timestamp = DateTime.Now;
            var chatTime = timestamp.ToString("HH:mm");

            var user = (ClaimsIdentity)Context.User.Identity;
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value;

            Clients.Group(roomId).broadcastMessage("TrollChat", userId + " joined to this channel (" + roomId + ")", chatTime);
        }

        public async Task LeaveRoom(string roomId)
        {
            await Groups.Remove(Context.ConnectionId, roomId);

            DateTime timestamp = DateTime.Now;
            var chatTime = timestamp.ToString("HH:mm");

            var user = (ClaimsIdentity)Context.User.Identity;
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value;

            Clients.Group(roomId).broadcastMessage("TrollChat", userId + " left from this channel (" + roomId + ")", chatTime);
        }

        public void Send(string roomId, string message)
        {
            if (string.IsNullOrEmpty(roomId.Trim()) || string.IsNullOrEmpty(message.Trim()))
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

                Clients.Group(roomId).broadcastMessage(user.Name.Trim(), message.Trim(), chatTime);
            }
        }
    }
}