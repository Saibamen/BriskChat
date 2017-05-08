using System;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class GetMessageById : IGetMessageById
    {
        private readonly IMessageRepository messageRepository;

        public GetMessageById(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public MessageModel Invoke(Guid id)
        {
            var dbMessage = messageRepository.GetById(id);

            if (dbMessage == null)
            {
                return null;
            }

            var message = AutoMapper.Mapper.Map<MessageModel>(dbMessage);

            return message;
        }
    }
}