using System;
using System.Collections.Generic;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class GetMessagesOffsetByRoomId : IGetMessagesOffsetByRoomId
    {
        private readonly IMessageRepository messageRepository;

        public GetMessagesOffsetByRoomId(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public List<MessageModel> Invoke(Guid roomId, int loadedMessagesIteration, int limit)
        {
            if (roomId == Guid.Empty)
            {
                return null;
            }

            var dbMessages = messageRepository.GetRoomMessagesOffset(roomId, loadedMessagesIteration, limit);

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