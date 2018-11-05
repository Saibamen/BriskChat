using System;
using BriskChat.BusinessLogic.Actions.Message.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Message.Implementations
{
    public class DeleteMessageById : IDeleteMessageById
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageById(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                return false;
            }

            var messageToDelete = _messageRepository.GetById(messageId);

            if (messageToDelete == null)
            {
                return false;
            }
            _messageRepository.Delete(messageToDelete);
            _unitOfWork.Save();

            return true;
        }
    }
}