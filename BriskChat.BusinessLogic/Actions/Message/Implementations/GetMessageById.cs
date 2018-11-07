using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.BusinessLogic.Actions.Message.Implementations
{
    public class GetMessageById : IGetMessageById
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessageById(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public MessageModel Invoke(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var dbMessage = _messageRepository
                .FindBy(x => x.Id == id)
                .Include(x => x.UserRoom.User)
                .FirstOrDefault();

            if (dbMessage == null)
            {
                return null;
            }

            var message = AutoMapper.Mapper.Map<MessageModel>(dbMessage);

            return message;
        }
    }
}