using System;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class EditMessageById : IEditMessageById
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditMessageById(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            this.messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid messageId, string messageText)
        {
            if (messageId == Guid.Empty || string.IsNullOrEmpty(messageText))
            {
                return false;
            }

            var messageToEdit = messageRepository.GetById(messageId);

            if (messageToEdit == null)
            {
                return false;
            }

            messageToEdit.Text = messageText;

            messageRepository.Edit(messageToEdit);
            _unitOfWork.Save();

            return true;
        }
    }
}