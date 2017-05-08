using System;
using System.Collections.Generic;
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
        private readonly IAddNewPrivateConversation addNewPrivateConversation;
        private readonly IGetUsersByDomainId getUsersByDomainId;
        private readonly IGetUserPrivateConversations getUserPrivateConversations;
        private readonly IDeleteMessageById deleteMessageById;
        private readonly IGetMessageById getMessageById;

        private const string TimeStampRepresentation = "HH:mm";

        public ChannelHub(IAddNewRoom addNewRoom,
            IAddNewMessage addNewMessage,
            IGetUserRoomByIds getUserRoomByIds,
            IGetUserRooms getUserRooms,
            IGetUsersByDomainId getUsersByDomainId,
            IAddNewPrivateConversation addNewPrivateConversation,
            IGetUserPrivateConversations getUserPrivateConversations,
            IDeleteMessageById deleteMessageById,
            IGetMessageById getMessageById)
        {
            this.addNewRoom = addNewRoom;
            this.addNewMessage = addNewMessage;
            this.getUserRoomByIds = getUserRoomByIds;
            this.getUserRooms = getUserRooms;
            this.getUsersByDomainId = getUsersByDomainId;
            this.addNewPrivateConversation = addNewPrivateConversation;
            this.getUserPrivateConversations = getUserPrivateConversations;
            this.deleteMessageById = deleteMessageById;
            this.getMessageById = getMessageById;
        }

        public void GetRooms()
        {
            var roomList = getUserRooms.Invoke(Context.UserId(), false);

            Clients.Caller.loadRooms(roomList);
        }

        public void GetPrivateConversations()
        {
            var roomList = getUserPrivateConversations.Invoke(Context.UserId());
            var viewList = new List<PrivateConversationViewModel>();

            foreach (var item in roomList)
            {
                viewList.Add(new PrivateConversationViewModel
                {
                    Id = item.Id,
                    UserName = item.User.Name,
                });
            }

            Clients.Caller.loadPrivateConversations(viewList);
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

            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), $"{Context.UserName()} joined to this channel ({roomId})", chatTime);
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

            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), $"{Context.UserName()} left this channel ({roomId})", chatTime);
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

            var dbMessageId = addNewMessage.Invoke(messageModel);

            if (dbMessageId != Guid.Empty)
            {
                Clients.Group(roomId).broadcastMessage(Context.UserName(), dbMessageId, message, chatTime);
            }
        }

        public void CreateNewChannel(CreateNewRoomViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return;
            }

            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);
            var room = addNewRoom.Invoke(roomModel, Context.UserId(), Context.DomainId());

            if (room != Guid.Empty)
            {
                Clients.Caller.channelAddedAction(model.Name, room, model.IsPublic);
            }
        }

        public void GetUsersFromDomain(string name)
        {
            var userList = getUsersByDomainId.Invoke(Context.DomainId());

            // Clients.Caller.privateConversationsUsersLoadedAction(userList);
        }

        public void CreateNewPrivateConversation(CreateNewPrivateConversationViewModel model)
        {
            model.Name = "private";
            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);
            var room = addNewPrivateConversation.Invoke(roomModel, Context.UserId(), model.SecondUserId);

            if (room != Guid.Empty)
            {
                Clients.Caller.privateConversationAddedAction(model.Name, room);
            }
        }

        public void EditMessage(string roomId, string messageId, string message)
        {
            if (string.IsNullOrEmpty(roomId.Trim()) || string.IsNullOrEmpty(messageId.Trim()) || string.IsNullOrEmpty(message.Trim()))
            {
                return;
            }

            // TODO: Check author and edit message in database

            Clients.Group(roomId).broadcastEditedMessage(messageId, message);
        }

        public void DeleteMessage(string roomId, string messageId)
        {
            if (string.IsNullOrEmpty(roomId.Trim()) || string.IsNullOrEmpty(messageId.Trim()))
            {
                return;
            }

            // TODO: Check author
            /*var message = getMessageById.Invoke(new Guid(messageId));

            if (message.UserRoom.User.Id != Context.UserId())
            {
                return;
            }*/

            var deleted = deleteMessageById.Invoke(new Guid(messageId));

            if (deleted)
            {
                Clients.Group(roomId).deleteMessage(messageId);
            }
        }
    }
}