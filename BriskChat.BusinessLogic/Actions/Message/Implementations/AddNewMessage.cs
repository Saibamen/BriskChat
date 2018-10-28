using System;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Message.Implementations
{
    public class AddNewMessage : IAddNewMessage
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewMessage(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            this.messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }

        public Guid Invoke(MessageModel message)
        {
            if (!message.IsValid())
            {
                return Guid.Empty;
            }

            var dbMessage = AutoMapper.Mapper.Map<DataAccess.Models.Message>(message);

            messageRepository.Add(dbMessage);
            _unitOfWork.Save();

            return dbMessage.Id;
        }
    }
}