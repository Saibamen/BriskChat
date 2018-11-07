using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Actions.UserRoom.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.Web.Helpers;
using BriskChat.Web.Models.Message;
using BriskChat.Web.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Profiling;

namespace BriskChat.Web.Hubs
{
    [Authorize(Roles = "User")]
    public class ChannelHub : Hub
    {
        private readonly IAddNewRoom _addNewRoom;
        private readonly IAddNewMessage _addNewMessage;
        private readonly IGetUserRooms _getUserRooms;
        private readonly IGetUserRoomByIds _getUserRoomByIds;
        private readonly IAddNewPrivateConversation _addNewPrivateConversation;
        private readonly IGetUsersByDomainId _getUsersByDomainId;
        private readonly IGetUserPrivateConversations _getUserPrivateConversations;
        private readonly IDeleteMessageById _deleteMessageById;
        private readonly IGetMessageById _getMessageById;
        private readonly IEditMessageById _editMessageById;
        private readonly IGetRoomUsers _getRoomUsers;
        private readonly IGetDomainPublicAndUserRooms _getDomainPublicAndUserRooms;
        private readonly IGetRoomInformation _getRoomInformation;
        private readonly IAddNewUserRoom _addNewUserRoom;
        private readonly IGetLastMessagesByRoomId _getLastMessagesByRoomId;
        private readonly IEditRoomCustomization _editRoomCustomization;
        private readonly IEditRoomName _editRoomName;
        private readonly IEditRoomDescription _editRoomDescription;
        private readonly IEditRoomTopic _editRoomTopic;
        private readonly IGetMessagesOffsetByRoomId _getMessagesOffsetByRoomId;
        private readonly IGetRoomUsersCount _getRoomUsersCount;
        private readonly IGetRoomByName _getRoomByName;
        private readonly IGetNotInvitedUsers _getNotInvitedUsers;

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
            IGetRoomUsersCount getRoomUsersCount,
            IGetRoomByName getRoomByName,
            IGetNotInvitedUsers getNotInvitedUsers)
        {
            _addNewRoom = addNewRoom;
            _addNewMessage = addNewMessage;
            _getUserRooms = getUserRooms;
            _getUserRoomByIds = getUserRoomByIds;
            _addNewPrivateConversation = addNewPrivateConversation;
            _getUsersByDomainId = getUsersByDomainId;
            _getUserPrivateConversations = getUserPrivateConversations;
            _deleteMessageById = deleteMessageById;
            _getMessageById = getMessageById;
            _editMessageById = editMessageById;
            _getRoomUsers = getRoomUsers;
            _getDomainPublicAndUserRooms = getDomainPublicAndUserRooms;
            _getRoomInformation = getRoomInformation;
            _addNewUserRoom = addNewUserRoom;
            _getLastMessagesByRoomId = getLastMessagesByRoomId;
            _editRoomCustomization = editRoomCustomization;
            _editRoomName = editRoomName;
            _editRoomDescription = editRoomDescription;
            _editRoomTopic = editRoomTopic;
            _getMessagesOffsetByRoomId = getMessagesOffsetByRoomId;
            _getRoomUsersCount = getRoomUsersCount;
            _getRoomByName = getRoomByName;
            _getNotInvitedUsers = getNotInvitedUsers;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.DomainId().ToString());
            ConnectedClients.Add(new UserConnection { ConnectionId = Context.ConnectionId, UserId = Context.UserId(), DomainId = Context.DomainId() });

            await Clients.Caller.SendAsync("setDomainInformation", Context.DomainName(), Context.UserName(), Context.UserId());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.DomainId().ToString());
            var userToDelete = ConnectedClients.FirstOrDefault(x => x.UserId == Context.UserId() && x.DomainId == Context.DomainId());
            ConnectedClients.Remove(userToDelete);

            await base.OnDisconnectedAsync(exception);
        }

        public void GetPreviousMessages(string roomId, string lastMessageId)
        {
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(lastMessageId))
            {
                return;
            }

            MiniProfiler.Current.Step("GetPreviousMessages");

            var messagesFromDb = _getMessagesOffsetByRoomId.Invoke(new Guid(roomId), new Guid(lastMessageId), MessagesToLoad);

            if (messagesFromDb == null || messagesFromDb.Count <= 0)
            {
                MiniProfiler.Current.Stop();
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

            Clients.Caller.SendAsync("parseOffsetMessages", viewList);
            MiniProfiler.Current.Stop();
        }

        public void GetRooms()
        {
            MiniProfiler.Current.Step("GetRooms");
            var roomList = _getUserRooms.Invoke(Context.UserId(), false);

            if (roomList.Count <= 0)
            {
                // TODO: Revisit here after domain refactoring
                var generalRoom = _getRoomByName.Invoke("general", Context.DomainId());

                if (generalRoom == null)
                {
                    var generalRoomModel = new RoomModel
                    {
                        Name = "general",
                        IsPublic = true,
                        IsPrivateConversation = false
                    };

                    var newGeneralRoomId = _addNewRoom.Invoke(generalRoomModel, Context.UserId(), Context.DomainId());
                    generalRoomModel.Id = newGeneralRoomId;

                    roomList.Add(generalRoomModel);
                }
                else
                {
                    var newUserRoom = _addNewUserRoom.Invoke(generalRoom.Id, new List<Guid> { Context.UserId() });

                    if (!newUserRoom)
                    {
                        MiniProfiler.Current.Stop();
                        return;
                    }

                    roomList.Add(generalRoom);
                }
            }

            Clients.Caller.SendAsync("loadRooms", roomList);

            MiniProfiler.Current.Stop();
        }

        public void GetDomainPublicAndUserRooms()
        {
            MiniProfiler.Current.Step("GetDomainPublicAndUserRooms");
            var roomList = _getDomainPublicAndUserRooms.Invoke(Context.DomainId(), Context.UserId());

            if (roomList.Count > 0)
            {
                var viewList = roomList.Select(item => new BrowseRoomsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsPublic = item.IsPublic,
                    Owner = item.Owner.Name,
                    Description = item.Description,
                    CreatedOn = item.CreatedOn.ToLocalTime().ToString(TimeStampRepresentationCreatedOn, CultureInfo.InvariantCulture)
                }).ToList();

                Clients.Caller.SendAsync("loadDomainPublicAndUserRooms", viewList);
            }

            MiniProfiler.Current.Stop();
        }

        public void GetPrivateConversations()
        {
            MiniProfiler.Current.Step("GetPrivateConversations");
            var roomList = _getUserPrivateConversations.Invoke(Context.UserId());

            if (roomList != null)
            {
                var viewList = roomList.Select(item => new PrivateConversationViewModel
                {
                    Id = item.Room.Id,
                    Name = StringSeparatorHelper.RemoveUserFromString(Context.UserName(), item.Room.Name)
                }).ToList();

                Clients.Caller.SendAsync("loadPrivateConversations", viewList);
            }

            MiniProfiler.Current.Stop();
        }

        public async Task JoinRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }

            MiniProfiler.Current.Step("JoinRoom");

            // Check if user has access to room (have userRoom in DB) and add if not (only on public rooms)
            var userRoom = _getUserRoomByIds.Invoke(new Guid(roomId), Context.UserId());

            if (userRoom == null)
            {
                var newUserRoom = _addNewUserRoom.Invoke(new Guid(roomId), new List<Guid> { Context.UserId() });

                if (!newUserRoom)
                {
                    MiniProfiler.Current.Stop();
                    return;
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            var roomUsersCount = _getRoomUsersCount.Invoke(new Guid(roomId));
            await Clients.Caller.SendAsync("updateRoomUsersCount", roomUsersCount);

            var messagesFromDb = _getLastMessagesByRoomId.Invoke(new Guid(roomId), MessagesToLoad);

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

                await Clients.Caller.SendAsync("parseLastMessages", viewList);
            }

            MiniProfiler.Current.Stop();
        }

        public async Task LeaveRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }

            MiniProfiler.Current.Step("LeaveRoom");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            MiniProfiler.Current.Stop();
        }

        public void SendMessage(string roomId, string message)
        {
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            message = message.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            MiniProfiler.Current.Step("SendMessage");

            var timestamp = DateTime.UtcNow;
            var chatTime = timestamp.ToLocalTime().ToString(TimeStampRepresentation, CultureInfo.InvariantCulture);

            // Add to database
            var userRoomModel = _getUserRoomByIds.Invoke(new Guid(roomId), Context.UserId());

            if (userRoomModel == null)
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var messageModel = new MessageModel
            {
                Text = message,
                CreatedOn = timestamp,
                UserRoom = userRoomModel
            };

            var dbMessageId = _addNewMessage.Invoke(messageModel);

            if (dbMessageId != Guid.Empty)
            {
                Clients.Group(roomId).SendAsync("broadcastMessage", Context.UserName(), Context.UserId(), dbMessageId, message, chatTime, GetMd5Hash(messageModel.UserRoom.User.Email));
            }

            MiniProfiler.Current.Stop();
        }

        public void CreateNewChannel(CreateNewRoomViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return;
            }

            MiniProfiler.Current.Step("CreateNewChannel");

            var roomModel = AutoMapper.Mapper.Map<RoomModel>(model);
            var room = _addNewRoom.Invoke(roomModel, Context.UserId(), Context.DomainId());

            if (room != Guid.Empty)
            {
                Clients.Caller.SendAsync("channelAddedAction", model.Name, room, model.IsPublic);
            }

            MiniProfiler.Current.Stop();
        }

        public static bool IsConnected(string connectionId, Guid userid)
        {
            return ConnectedClients.Any(x => x.UserId == userid);
        }

        public static string GetMd5Hash(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
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
            MiniProfiler.Current.Step("GetUsersFromDomain");
            var userList = _getUsersByDomainId.Invoke(Context.DomainId());
            userList.Remove(userList.FirstOrDefault(x => x.Id == Context.UserId()));

            if (userList.Count > 0)
            {
                var viewList = userList.Select(item => new UserViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsOnline = IsConnected(Context.ConnectionId, item.Id),
                    EmailHash = GetMd5Hash(item.Email)
                });

                Clients.Caller.SendAsync("privateConversationsUsersLoadedAction", viewList);
            }

            MiniProfiler.Current.Stop();
        }

        public void GetNotInvitedUsers(string roomId)
        {
            MiniProfiler.Current.Step("GetNotInvitedUsers");
            var userList = _getNotInvitedUsers.Invoke(Context.DomainId(), new Guid(roomId));
            userList.Remove(userList.FirstOrDefault(x => x.Id == Context.UserId()));

            if (userList.Count > 0)
            {
                var viewList = userList.Select(item => new UserViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsOnline = IsConnected(Context.ConnectionId, item.Id),
                    EmailHash = GetMd5Hash(item.Email)
                });

                Clients.Caller.SendAsync("notInvitedUsersLoadedAction", viewList);
            }

            MiniProfiler.Current.Stop();
        }

        public void GetRoomInformation(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }

            MiniProfiler.Current.Step("GetRoomInformation");

            var roomInformation = _getRoomInformation.Invoke(new Guid(roomId));
            var informationR = AutoMapper.Mapper.Map<GetRoomInformationViewModel>(roomInformation);
            var createdOn = informationR.CreatedOn.ToString(TimeStampRepresentationCreatedOn, CultureInfo.InvariantCulture);

            Clients.Caller.SendAsync("RoomInfo", informationR, createdOn);
            MiniProfiler.Current.Stop();
        }

        public void GetRoomUsers(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }

            MiniProfiler.Current.Step("GetRoomUsers");

            var roomUserList = _getRoomUsers.Invoke(new Guid(roomId));

            var userList = roomUserList.Select(item => new UserViewModel
            {
                Id = item.Id,
                Name = item.Name,
                IsOnline = IsConnected(Context.ConnectionId, item.Id),
                EmailHash = GetMd5Hash(item.Email)
            });

            Clients.Caller.SendAsync("UsersInRoom", userList);
            MiniProfiler.Current.Stop();
        }

        public void CreateNewPrivateConversation(List<Guid> users)
        {
            MiniProfiler.Current.Step("CreateNewPrivateConversation");
            // If list has duplicates - abort!
            if (users.Count <= 0 || users.Count > 8 || users.Distinct().Count() != users.Count)
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var room = _addNewPrivateConversation.Invoke(Context.UserId(), users);

            if (room == null)
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var tempName = StringSeparatorHelper.RemoveUserFromString(Context.UserName(), room.Name);
            Clients.Caller.SendAsync("privateConversationAddedAction", new PrivateConversationViewModel { Id = room.Id, Name = tempName });
            MiniProfiler.Current.Stop();
        }

        public void InviteUsersToPrivateRoom(string roomId, List<Guid> users)
        {
            MiniProfiler.Current.Step("InviteUsersToPrivateRoom");
            // If list has duplicates - abort!
            if (users.Count <= 0 || users.Distinct().Count() != users.Count)
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var added = _addNewUserRoom.Invoke(new Guid(roomId), users, true);

            if (!added)
            {
                MiniProfiler.Current.Stop();
                return;
            }

            Clients.Caller.SendAsync("inviteUsersAddedAction");
            MiniProfiler.Current.Stop();
        }

        public void EditMessage(string roomId, string messageId, string messageText)
        {
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(messageId) || string.IsNullOrWhiteSpace(messageText))
            {
                return;
            }

            messageText = messageText.Trim();

            if (string.IsNullOrWhiteSpace(messageText))
            {
                return;
            }

            MiniProfiler.Current.Step("EditMessage");

            var messageFromDb = _getMessageById.Invoke(new Guid(messageId));

            if (messageFromDb == null || messageFromDb.UserRoom.User.Id != Context.UserId())
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var edited = _editMessageById.Invoke(new Guid(messageId), messageText);

            if (edited)
            {
                Clients.Group(roomId).SendAsync("broadcastEditedMessage", messageId, messageText);
            }

            MiniProfiler.Current.Stop();
        }

        public void EditRoomCustomization(string roomId, int roomCustomization)
        {
            if (string.IsNullOrWhiteSpace(roomId) || roomCustomization < 0)
            {
                return;
            }

            MiniProfiler.Current.Step("EditRoomCustomization");

            var edited = _editRoomCustomization.Invoke(new Guid(roomId), roomCustomization);

            if (edited)
            {
                Clients.Group(roomId).SendAsync("broadcastEditedRoomCustomization", roomCustomization);
            }

            MiniProfiler.Current.Stop();
        }

        public void EditRoomName(string roomId, string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(roomName) || roomName.Length > 100)
            {
                return;
            }

            MiniProfiler.Current.Step("EditRoomName");

            var edited = _editRoomName.Invoke(new Guid(roomId), roomName);

            if (edited)
            {
                Clients.All.SendAsync("broadcastDomainEditedRoomName", roomId, roomName);
                Clients.Group(roomId).SendAsync("broadcastEditedActiveRoomName", roomName);
            }

            MiniProfiler.Current.Stop();
        }

        public void EditRoomDescription(string roomId, string roomDescription)
        {
            if (string.IsNullOrWhiteSpace(roomId) || roomDescription.Length > 100)
            {
                return;
            }

            MiniProfiler.Current.Step("EditRoomDescription");

            var edited = _editRoomDescription.Invoke(new Guid(roomId), roomDescription);

            if (edited)
            {
                Clients.Group(roomId).SendAsync("broadcastEditedRoomDescription", roomDescription);
            }

            MiniProfiler.Current.Stop();
        }

        public void EditRoomTopic(string roomId, string roomTopic)
        {
            if (string.IsNullOrWhiteSpace(roomId) || roomTopic.Length > 100)
            {
                return;
            }

            MiniProfiler.Current.Step("EditRoomTopic");

            var edited = _editRoomTopic.Invoke(new Guid(roomId), roomTopic);

            if (edited)
            {
                Clients.Group(roomId).SendAsync("broadcastEditedRoomTopic", roomTopic);
            }

            MiniProfiler.Current.Stop();
        }

        public void DeleteMessage(string roomId, string messageId)
        {
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(messageId))
            {
                return;
            }

            MiniProfiler.Current.Step("DeleteMessage");

            var message = _getMessageById.Invoke(new Guid(messageId));

            if (message == null || message.UserRoom.User.Id != Context.UserId())
            {
                MiniProfiler.Current.Stop();
                return;
            }

            var deleted = _deleteMessageById.Invoke(new Guid(messageId));

            if (deleted)
            {
                Clients.Group(roomId).SendAsync("deleteMessage", messageId);
            }

            MiniProfiler.Current.Stop();
        }
    }
}