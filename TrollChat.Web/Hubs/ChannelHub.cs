﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Profiling;
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
        private readonly IGetDomainPublicAndUserRooms getDomainPublicAndUserRooms;
        private readonly IGetRoomInformation getRoomInformation;
        private readonly IAddNewUserRoom addNewUserRoom;
        private readonly IGetLastMessagesByRoomId getLastMessagesByRoomId;
        private readonly IEditRoomCustomization editRoomCustomization;
        private readonly IEditRoomName editRoomName;
        private readonly IEditRoomDescription editRoomDescription;
        private readonly IEditRoomTopic editRoomTopic;
        private readonly IGetMessagesOffsetByRoomId getMessagesOffsetByRoomId;
        private readonly IGetRoomUsersCount getRoomUsersCount;

        private const string TimeStampRepresentation = "HH:mm";
        private const string TimeStampRepresentationCreatedOn = "MMMM d, yyyy";
        private static readonly List<UserConnection> ConnectedClients = new List<UserConnection>();
        private const int MessagesToLoad = 20;

        public ChannelHub(IAddNewRoom addNewRoom,
            IAddNewMessage addNewMessage,
            IGetUserRooms getUserRooms,
            IGetUserRoomByIds getUserRoomByIds,
            IAddNewPrivateConversation addNewPrivateConversation,
            IGetUsersByDomainId getUsersByDomainId,
            IGetUserPrivateConversations getUserPrivateConversations,
            IDeleteMessageById deleteMessageById,
            IGetMessageById getMessageById,
            IEditMessageById editMessageById,
            IGetRoomUsers getRoomUsers,
            IGetDomainPublicAndUserRooms getDomainPublicAndUserRooms,
            IGetRoomInformation getRoomInformation,
            IAddNewUserRoom addNewUserRoom,
            IGetLastMessagesByRoomId getLastMessagesByRoomId,
            IEditRoomCustomization editRoomCustomization,
            IEditRoomName editRoomName,
            IEditRoomDescription editRoomDescription,
            IEditRoomTopic editRoomTopic,
            IGetMessagesOffsetByRoomId getMessagesOffsetByRoomId,
            IGetRoomUsersCount getRoomUsersCount)
        {
            this.addNewRoom = addNewRoom;
            this.addNewMessage = addNewMessage;
            this.getUserRooms = getUserRooms;
            this.getUserRoomByIds = getUserRoomByIds;
            this.addNewPrivateConversation = addNewPrivateConversation;
            this.getUsersByDomainId = getUsersByDomainId;
            this.getUserPrivateConversations = getUserPrivateConversations;
            this.deleteMessageById = deleteMessageById;
            this.getMessageById = getMessageById;
            this.editMessageById = editMessageById;
            this.getRoomUsers = getRoomUsers;
            this.getDomainPublicAndUserRooms = getDomainPublicAndUserRooms;
            this.getRoomInformation = getRoomInformation;
            this.addNewUserRoom = addNewUserRoom;
            this.getLastMessagesByRoomId = getLastMessagesByRoomId;
            this.editRoomCustomization = editRoomCustomization;
            this.editRoomName = editRoomName;
            this.editRoomDescription = editRoomDescription;
            this.editRoomTopic = editRoomTopic;
            this.getMessagesOffsetByRoomId = getMessagesOffsetByRoomId;
            this.getRoomUsersCount = getRoomUsersCount;
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

        public void GetPreviousMessages(string roomId, string lastMessageId)
        {
            MiniProfiler.Start("GetPreviousMessages");
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(lastMessageId))
            {
                return;
            }

            var messagesFromDb = getMessagesOffsetByRoomId.Invoke(new Guid(roomId), new Guid(lastMessageId), MessagesToLoad);

            if (messagesFromDb == null || messagesFromDb.Count <= 0)
            {
                return;
            }

            var viewList = messagesFromDb.Select(item => new MessageViewModel
            {
                Id = item.Id,
                UserName = item.UserRoom.User.Name,
                UserId = item.UserRoom.User.Id,
                Text = item.Text,
                CreatedOn = item.CreatedOn.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture),
                EmailHash = GetMd5Hash(item.UserRoom.User.Email)
            });

            Clients.Caller.parseOffsetMessages(viewList);
            MiniProfiler.Stop();
        }

        public void GetRooms()
        {
            MiniProfiler.Start("GetRooms");
            var roomList = getUserRooms.Invoke(Context.UserId(), false);

            if (roomList != null)
            {
                Clients.Caller.loadRooms(roomList);
            }
            // FIXME: Auto add user to general channel
            MiniProfiler.Stop();
        }

        public void GetDomainPublicRooms()
        {
            MiniProfiler.Start("GetDomainPublicRooms");
            var roomList = getDomainPublicAndUserRooms.Invoke(Context.DomainId(), Context.UserId());
            var viewList = new List<BrowseRoomsViewModel>();

            foreach (var item in roomList)
            {
                var newItem = new BrowseRoomsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsPublic = item.IsPublic,
                    Owner = item.Owner.Name,
                    Description = item.Description,
                    CreatedOn = item.CreatedOn.ToLocalTime().ToString(TimeStampRepresentationCreatedOn, CultureInfo.InvariantCulture)
                };

                viewList.Add(newItem);
            }

            Clients.Caller.loadDomainPublicRooms(viewList);
            MiniProfiler.Stop();
        }

        public void GetPrivateConversations()
        {
            MiniProfiler.Start("GetPrivateConversations");
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
            MiniProfiler.Stop();
        }

        public async Task JoinRoom(string roomId)
        {
            MiniProfiler.Start("JoinRoom");
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

            var roomUsersCount = getRoomUsersCount.Invoke(new Guid(roomId));
            Clients.Caller.updateRoomUsersCount(roomUsersCount);

            var messagesFromDb = getLastMessagesByRoomId.Invoke(new Guid(roomId), MessagesToLoad);

            if (messagesFromDb != null && messagesFromDb.Count > 0)
            {
                var viewList = messagesFromDb.Select(item => new MessageViewModel
                {
                    Id = item.Id,
                    UserName = item.UserRoom.User.Name,
                    UserId = item.UserRoom.User.Id,
                    Text = item.Text,
                    CreatedOn = item.CreatedOn.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture),
                    EmailHash = GetMd5Hash(item.UserRoom.User.Email)
                });

                Clients.Caller.parseLastMessages(viewList);
            }

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture);

            // DEBUG
            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), new Guid(), $"{Context.UserName()} joined to this channel ({roomId})", chatTime);
            MiniProfiler.Stop();
        }

        public async Task LeaveRoom(string roomId)
        {
            MiniProfiler.Start("LeaveRoom");
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            await Groups.Remove(Context.ConnectionId, roomId);

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture);

            // DEBUG
            Clients.Group(roomId).broadcastMessage("TrollChat", new Guid(), new Guid(), $"{Context.UserName()} left this channel ({roomId})", chatTime);
            MiniProfiler.Stop();
        }

        public void SendMessage(string roomId, string message)
        {
            MiniProfiler.Start("SendMessage");
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
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture);

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
                Clients.Group(roomId).broadcastMessage(Context.UserName(), Context.UserId(), dbMessageId, message, chatTime, GetMd5Hash(messageModel.UserRoom.User.Email));
            }
            MiniProfiler.Stop();
        }

        public void CreateNewChannel(CreateNewRoomViewModel model)
        {
            MiniProfiler.Start("CreateNewChannel");
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
            MiniProfiler.Stop();
        }

        public static bool IsConnected(string connectionid, Guid userid)
        {
            return ConnectedClients.Any(x => x.UserId == userid);
        }

        public static string GetMd5Hash(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var encoder = new UTF8Encoding();
            var hasher = MD5.Create();

            var hashedBytes = hasher.ComputeHash(encoder.GetBytes(email.ToLower()));
            var sb = new StringBuilder(hashedBytes.Length * 2);

            foreach (var t in hashedBytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();
        }

        public void GetUsersFromDomain()
        {
            MiniProfiler.Start("GetUsersFromDomain");
            var userList = getUsersByDomainId.Invoke(Context.DomainId());
            userList.Remove(userList.FirstOrDefault(x => x.Id == Context.UserId()));

            var viewList = userList.Select(item => new UserViewModel
            {
                Id = item.Id,
                Name = item.Name,
                IsOnline = IsConnected(Context.ConnectionId, item.Id),
                EmailHash = GetMd5Hash(item.Email)
            });

            Clients.Caller.privateConversationsUsersLoadedAction(viewList);
            MiniProfiler.Stop();
        }

        public void GetRoomInformation(string roomId)
        {
            MiniProfiler.Start("GetRoomInformation");
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var roomInformation = getRoomInformation.Invoke(new Guid(roomId));
            var informationR = AutoMapper.Mapper.Map<GetRoomInformationViewModel>(roomInformation);
            var createdOn = informationR.CreatedOn.ToString(TimeStampRepresentationCreatedOn, CultureInfo.InvariantCulture);

            Clients.Caller.RoomInfo(informationR, createdOn);
            MiniProfiler.Stop();
        }

        public void GetRoomUsers(string roomId)
        {
            MiniProfiler.Start("GetRoomUsers");
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            var roomUserList = getRoomUsers.Invoke(new Guid(roomId));

            var userList = roomUserList.Select(item => new UserViewModel
            {
                Id = item.Id,
                Name = item.Name,
                IsOnline = IsConnected(Context.ConnectionId, item.Id),
                EmailHash = GetMd5Hash(item.Email)
            });

            Clients.Caller.UsersInRoom(userList);
            MiniProfiler.Stop();
        }

        public void CreateNewPrivateConversation(List<Guid> model)
        {
            MiniProfiler.Start("CreateNewPrivateConversation");
            // If list has duplicates abort!
            if (model.Count <= 0 || model.Distinct().Count() != model.Count)
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
            MiniProfiler.Stop();
        }

        public void EditMessage(string roomId, string messageId, string messageText)
        {
            MiniProfiler.Start("EditMessage");
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
            MiniProfiler.Stop();
        }

        public void EditRoomCustomization(string roomId, int roomCustomization)
        {
            MiniProfiler.Start("EditRoomCustomization");
            if (string.IsNullOrEmpty(roomId) || roomCustomization < 0)
            {
                return;
            }

            var edited = editRoomCustomization.Invoke(new Guid(roomId), roomCustomization);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomCustomization(roomCustomization);
            }
            MiniProfiler.Stop();
        }

        public void EditRoomName(string roomId, string roomName)
        {
            MiniProfiler.Start("EditRoomName");
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(roomName) || roomName.Length > 100)
            {
                return;
            }

            var edited = editRoomName.Invoke(new Guid(roomId), roomName);

            if (edited)
            {
                Clients.All.broadcastDomainEditedRoomName(roomId, roomName);
                Clients.Group(roomId).broadcastEditedActiveRoomName(roomName);
            }
            MiniProfiler.Stop();
        }

        public void EditRoomDescription(string roomId, string roomDescription)
        {
            MiniProfiler.Start("EditRoomDescription");
            if (string.IsNullOrEmpty(roomId) || roomDescription.Length > 100)
            {
                return;
            }

            var edited = editRoomDescription.Invoke(new Guid(roomId), roomDescription);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomDescription(roomDescription);
            }
            MiniProfiler.Stop();
        }

        public void EditRoomTopic(string roomId, string roomTopic)
        {
            MiniProfiler.Start("EditRoomTopic");
            if (string.IsNullOrEmpty(roomId) || roomTopic.Length > 100)
            {
                return;
            }

            var edited = editRoomTopic.Invoke(new Guid(roomId), roomTopic);

            if (edited)
            {
                Clients.Group(roomId).broadcastEditedRoomTopic(roomTopic);
            }
            MiniProfiler.Stop();
        }

        public void DeleteMessage(string roomId, string messageId)
        {
            MiniProfiler.Start("DeleteMessage");
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
            MiniProfiler.Stop();
        }
    }
}