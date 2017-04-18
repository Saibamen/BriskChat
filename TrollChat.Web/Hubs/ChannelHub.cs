using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Models.Room;

namespace TrollChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        private readonly IGetUserById getUserById;
        private readonly IAddNewRoom addNewRoom;

        public ChannelHub(IGetUserById getUserById, IAddNewRoom addNewRoom)
        {
            this.addNewRoom = addNewRoom;
            this.getUserById = getUserById;
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.Add(Context.ConnectionId, roomId);

            DateTime time = DateTime.UtcNow;
            var localTime = time.ToLocalTime();

            var user = (ClaimsIdentity)Context.User.Identity;
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value;

            Clients.Group(roomId).broadcastMessage("TrollChat", userId + " joined to this channel (" + roomId + ")", localTime);
        }

        public async Task LeaveRoom(string roomId)
        {
            await Groups.Remove(Context.ConnectionId, roomId);

            DateTime time = DateTime.UtcNow;
            var localTime = time.ToLocalTime();

            var user = (ClaimsIdentity)Context.User.Identity;
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value;

            Clients.Group(roomId).broadcastMessage("TrollChat", userId + " left from this channel (" + roomId + ")", localTime);
        }

        public void Send(string roomId, string message)
        {
            if (string.IsNullOrEmpty(roomId.Trim()) || string.IsNullOrEmpty(message.Trim()))
            {
                return;
            }

            var userClaims = (ClaimsIdentity)Context.User.Identity;
            var userId = Int32.Parse(userClaims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            var user = getUserById.Invoke(userId);

            if (user != null)
            {
                DateTime time = DateTime.UtcNow;
                var localTime = time.ToLocalTime();

                // TODO: Save to database

                Clients.Group(roomId).broadcastMessage(user.Name.Trim(), message.Trim(), localTime);
            }
        }

        public void CreateNewChannel(CreateNewRoomViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return;
            }

            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);

            //TODO: change to real id
            var room = addNewRoom.Invoke(roomModel, 1);

            if (room == 0)
            {
                return;
            }

            Clients.Caller.channelAddedAction(model.Name);
        }
    }
}