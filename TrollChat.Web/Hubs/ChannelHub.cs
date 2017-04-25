using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.UserRoom.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Helpers;
using TrollChat.Web.Models.Room;

namespace TrollChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        private readonly IAddNewRoom addNewRoom;
        private readonly IAddNewMessage addNewMessage;
        private readonly IGetUserRooms getUserRooms;
        private readonly IGetUserRoomByIds getUserRoomByIds;

        private readonly IGetUsersByDomainId getUsersByDomainId;
        private const string TimeStampRepresentation = "HH:mm";

        public ChannelHub(IAddNewRoom addNewRoom,
            IAddNewMessage addNewMessage,
            IGetUserRoomByIds getUserRoomByIds,
            IGetUserRooms getUserRooms,
            IGetUsersByDomainId getUsersByDomainId)
        {
            this.addNewRoom = addNewRoom;
            this.addNewMessage = addNewMessage;
            this.getUserRoomByIds = getUserRoomByIds;
            this.getUserRooms = getUserRooms;
            this.getUsersByDomainId = getUsersByDomainId;
        }

        public void GetRooms()
        {
            var roomList = getUserRooms.Invoke(Context.UserId(), false);

            Clients.Caller.loadRooms(roomList);
        }

        public void GetPrivateConversations()
        {
            var roomList = getUserRooms.Invoke(Context.UserId(), true);

            Clients.Caller.loadPrivateConversations(roomList);
        }

        public async Task JoinRoom(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            await Groups.Add(Context.ConnectionId, roomId);

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation);

            Clients.Group(roomId).broadcastMessage("TrollChat", $"{Context.UserName()} joined to this channel ({roomId})", chatTime);
        }

        public async Task LeaveRoom(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            await Groups.Remove(Context.ConnectionId, roomId);

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation);

            Clients.Group(roomId).broadcastMessage("TrollChat", $"{Context.UserName()} left this channel ({roomId})", chatTime);
        }

        public void SendMessage(string roomId, string message)
        {
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(message))
            {
                return;
            }

            message = message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation);

            // Add to database
            var userRoomModel = getUserRoomByIds.Invoke(new Guid(roomId), Context.UserId());

            var messageModel = new MessageModel
            {
                Text = message,
                CreatedOn = timestamp,
                UserRoom = userRoomModel
            };

            var dbMessage = addNewMessage.Invoke(messageModel);

            if (dbMessage)
            {
                Clients.Group(roomId).broadcastMessage(Context.UserName(), message, chatTime);
            }
        }

        public void CreateNewChannel(CreateNewRoomViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return;
            }

            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);
            var room = addNewRoom.Invoke(roomModel, Context.UserId());

            if (room == Guid.Empty)
            {
                return;
            }

            Clients.Caller.channelAddedAction(model.Name, room, model.IsPublic);
        }

        public void GetUsersFromDomain(string name)
        {
            getUsersByDomainId.Invoke(Context.DomainId());
            return;
        }

        public void CreateNewPrivateConversation(CreateNewPrivateConversationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return;
            }

            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);
            var room = addNewRoom.Invoke(roomModel, Context.UserId());

            if (room == Guid.Empty)
            {
                return;
            }

            //TODO: Add private conversation to sidebar
            //  Clients.Caller.channelAddedAction(model.Name, room, model.IsPublic);
        }

        public void DeleteMessage(string roomId, int messageId)
        {
            if (string.IsNullOrEmpty(roomId.Trim()) || messageId <= 0)
            {
                return;
            }

            // TODO: Check author and delete from database

            Clients.Group(roomId).deleteMessage(messageId);
        }
    }
}