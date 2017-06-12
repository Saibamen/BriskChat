using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.BusinessLogic.Actions.Room.Interfaces;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.UserRoom.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Helpers;
using TrollChat.Web.Models.Message;
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
        private readonly IEditMessageById editMessageById;
        private readonly IGetRoomUsers getRoomUsers;
        private readonly IGetDomainPublicRooms getDomainPublicRooms;
        private readonly IGetRoomInformation getRoomInformation;
        private readonly IAddNewUserRoom addNewUserRoom;
        private readonly IGetLastMessagesByRoomId getLastMessagesByRoomId;
        private readonly IEditRoomCustomization editRoomCustomization;
        private readonly IEditRoomName editRoomName;
        private readonly IEditRoomDescription editRoomDescription;

        private const string TimeStampRepresentation = "HH:mm";
        private const string TimeStampRepresentationCreatedOn = "HH:mm dd-MM-yyyy";
        private static readonly List<UserConnection> ConnectedClients = new List<UserConnection>();
        private const int MessagesToLoad = 20;

        public ChannelHub(IAddNewRoom addNewRoom,
            IAddNewMessage addNewMessage,
            IGetUserRoomByIds getUserRoomByIds,
            IGetUserRooms getUserRooms,
            IGetUsersByDomainId getUsersByDomainId,
            IAddNewPrivateConversation addNewPrivateConversation,
            IGetUserPrivateConversations getUserPrivateConversations,
            IDeleteMessageById deleteMessageById,
            IGetMessageById getMessageById,
            IEditMessageById editMessageById,
            IGetRoomUsers getRoomUsers,
            IGetDomainPublicRooms getDomainPublicRooms,
            IGetRoomInformation getRoomInformation,
            IAddNewUserRoom addNewUserRoom,
            IGetLastMessagesByRoomId getLastMessagesByRoomId,
            IEditRoomCustomization editRoomCustomization,
            IEditRoomDescription editRoomDescription,
            IEditRoomName editRoomName)
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
            this.editMessageById = editMessageById;
            this.getRoomUsers = getRoomUsers;
            this.getDomainPublicRooms = getDomainPublicRooms;
            this.getRoomInformation = getRoomInformation;
            this.addNewUserRoom = addNewUserRoom;
            this.getLastMessagesByRoomId = getLastMessagesByRoomId;
            this.editRoomCustomization = editRoomCustomization;
            this.editRoomDescription = editRoomDescription;
            this.editRoomName = editRoomName;
        }

        public override Task OnConnected()
        {
            Groups.Add(Context.ConnectionId, Context.DomainId().ToString());
            ConnectedClients.Add(new UserConnection { ConnectionId = Context.ConnectionId, UserId = Context.UserId(), DomainId = Context.DomainId() });

            Clients.Caller.setDomainInformation(Context.DomainName(), Context.UserName(), Context.UserId());

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Groups.Remove(Context.ConnectionId, Context.DomainId().ToString());
            var userToDelete = ConnectedClients.FirstOrDefault(x => x.UserId == Context.UserId() && x.DomainId == Context.DomainId());
            ConnectedClients.Remove(userToDelete);

            return base.OnDisconnected(stopCalled);
        }

        public void GetRooms()
        {
            var roomList = getUserRooms.Invoke(Context.UserId(), false);

            Clients.Caller.loadRooms(roomList);
        }

        public void GetDomainPublicRooms()
        {
            var roomList = getDomainPublicRooms.Invoke(Context.DomainId());

            Clients.Caller.loadDomainPublicRooms(roomList);
        }

        public void GetPrivateConversations()
        {
            var roomList = getUserPrivateConversations.Invoke(Context.UserId());
            var viewList = new List<PrivateConversationViewModel>();

            foreach (var item in roomList)
            {
                var newItem = new PrivateConversationViewModel
                {
                    Id = item.Room.Id,
                    Name = StringSeparatorHelper.RemoveUserFromString(Context.UserName(), item.Room.Name)
                };

                viewList.Add(newItem);
            }

            Clients.Caller.loadPrivateConversations(viewList);
        }

        public async Task JoinRoom(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            // Check if user has access to room (have userRoom in DB) and add if not (only on public rooms)
            var userRoom = getUserRoomByIds.Invoke(new Guid(roomId), Context.UserId());

            if (userRoom == null)
            {
                var newUserRoom = addNewUserRoom.Invoke(new Guid(roomId), Context.UserId());

                if (!newUserRoom)
                {
                    return;
                }
            }

            await Groups.Add(Context.ConnectionId, roomId);

            var messagesFromDb = getLastMessagesByRoomId.Invoke(new Guid(roomId), MessagesToLoad);

            var viewList = messagesFromDb.Select(item => new MessageViewModel
            {
                Id = item.Id,
                UserName = item.UserRoom.User.Name,
                UserId = item.UserRoom.User.Id,
                Text = item.Text,
                CreatedOn = item.CreatedOn.ToLocalTime().ToString(TimeStampRepresentation)
            });

            Clients.Caller.parseLastMessages(viewList);

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation);

            // DEBUG
            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), new Guid(), $"{Context.UserName()} joined to this channel ({roomId})", chatTime);
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

            // DEBUG
            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), new Guid(), $"{Context.UserName()} left this channel ({roomId})", chatTime);
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

            if (userRoomModel == null)
            {
                return;
            }

            var messageModel = new MessageModel
            {
                Text = message,
                CreatedOn = timestamp,
                UserRoom = userRoomModel
            };

            var dbMessageId = addNewMessage.Invoke(messageModel);

            if (dbMessageId != Guid.Empty)
            {
                Clients.Group(roomId).broadcastMessage(Context.UserName(), Context.UserId(), dbMessageId, message, chatTime);
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

        public static bool IsConnected(string connectionid, Guid userid)
        {
            return ConnectedClients.Any(x => x.UserId == userid);
        }

        public void GetUsersFromDomain()
        {
            var userList = getUsersByDomainId.Invoke(Context.DomainId());
            userList.Remove(userList.FirstOrDefault(x => x.Id == Context.UserId()));

            var viewList = userList.Select(item => new PrivateConversationUserViewModel
            {
                Id = item.Id,
                UserName = item.Email,
                Name = item.Name,
                IsOnline = IsConnected(Context.ConnectionId, item.Id)
            });

            Clients.Caller.privateConversationsUsersLoadedAction(viewList);
        }

        public void GetRoomInformation(string roomId)
        {
            var roominformation = getRoomInformation.Invoke(new Guid(roomId));
            var informationR = AutoMapper.Mapper.Map<GetRoomInformationViewModel>(roominformation);
            var createdOn = informationR.CreatedOn.ToLocalTime().ToString(TimeStampRepresentationCreatedOn);

            Clients.Caller.RoomInfo(informationR, createdOn);
        }

        public void GetRoomUsers(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var roomUserList = getRoomUsers.Invoke(new Guid(roomId));
            roomUserList.Remove(roomUserList.FirstOrDefault(x => x.Id == new Guid(roomId)));

            var userList = roomUserList.Select(item => new RoomUsersViewModel
            {
                UserId = item.Id,
                Name = item.Name,
                Email = item.Email
            });

            if (new Guid(roomId) == Guid.Empty)
            {
                return;
            }

            Clients.Caller.UsersInRoom(userList);
        }

        public void CreateNewPrivateConversation(List<Guid> model)
        {
            // If list has duplicates abort!
            if (model.Distinct().Count() != model.Count)
            {
                return;
            }

            var room = addNewPrivateConversation.Invoke(Context.UserId(), model);

            if (room == null)
            {
                return;
            }

            var tempName = StringSeparatorHelper.RemoveUserFromString(Context.UserName(), room.Name);
            Clients.Caller.privateConversationAddedAction(new PrivateConversationViewModel { Id = room.Id, Name = tempName });
        }

        public void EditMessage(string roomId, string messageId, string messageText)
        {
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(messageId) || string.IsNullOrEmpty(messageText))
            {
                return;
            }

            messageText = messageText.Trim();

            if (string.IsNullOrEmpty(messageText))
            {
                return;
            }

            var messageFromDb = getMessageById.Invoke(new Guid(messageId));

            if (messageFromDb == null || messageFromDb.UserRoom.User.Id != Context.UserId())
            {
                return;
            }

            var edited = editMessageById.Invoke(new Guid(messageId), messageText);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedMessage(messageId, messageText);
            }
        }

        public void EditRoomCustomization(string roomId, int roomCustomization)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var edited = editRoomCustomization.Invoke(new Guid(roomId), roomCustomization);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomCustomnization(roomId, roomCustomization);
            }
        }

        public void EditRoomName(string roomId, string roomName)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var edited = editRoomName.Invoke(new Guid(roomId), roomName);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomName(roomId, roomName);
            }
        }

        public void EditRoomDescription(string roomId, string roomDescription)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var edited = editRoomDescription.Invoke(new Guid(roomId), roomDescription);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomDescription(roomId, roomDescription);
            }
        }

        public void DeleteMessage(string roomId, string messageId)
        {
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(messageId))
            {
                return;
            }

            var message = getMessageById.Invoke(new Guid(messageId));

            if (message == null || message.UserRoom.User.Id != Context.UserId())
            {
                return;
            }

            var deleted = deleteMessageById.Invoke(new Guid(messageId));

            if (deleted)
            {
                Clients.Group(roomId).deleteMessage(messageId);
            }
        }
    }
}