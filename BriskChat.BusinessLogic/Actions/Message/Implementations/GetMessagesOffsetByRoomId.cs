using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Message.Implementations
{
    public class GetMessagesOffsetByRoomId : IGetMessagesOffsetByRoomId
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessagesOffsetByRoomId(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public List<MessageModel> Invoke(Guid roomId, Guid lastMessageId, int limit)
        {
            if (roomId == Guid.Empty || lastMessageId == Guid.Empty || limit < 1)
            {
                return null;
            }

            var lastMessage = _messageRepository.GetById(lastMessageId);

            if (lastMessage == null)
            {
                return null;
            }

            var dbMessages = _messageRepository.GetRoomMessagesOffset(roomId, lastMessage.CreatedOn, limit);

            if (dbMessages.Count() <= 0)
            {
                return null;
            }

            var messageList = new List<MessageModel>();

            foreach (var item in dbMessages)
            {
                messageList.Add(new MessageModel
                {
                    Id = item.Id,
                    Text = item.Text,
                    CreatedOn = item.CreatedOn,
                    UserRoom = new UserRoomModel
                    {
                        User = AutoMapper.Mapper.Map<UserModel>(item.UserRoom.User)
                    }
                });
            }

            return messageList;
        }
    }
}