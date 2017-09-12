using System;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class EditMessageById : IEditMessageById
    {
        private readonly IMessageRepository messageRepository;

        public EditMessageById(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
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
            messageRepository.Save();

            return true;
        }
    }
}