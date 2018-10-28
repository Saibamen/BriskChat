using System;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Message.Implementations
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
            if (id == Guid.Empty)
            {
                return null;
            }

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