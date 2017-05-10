using System;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class DeleteMessageById : IDeleteMessageById
    {
        private readonly IMessageRepository messageRepository;

        public DeleteMessageById(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public bool Invoke(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                return false;
            }

            var messageToDelete = messageRepository.GetById(messageId);

            if (messageToDelete == null)
            {
                return false;
            }

            messageRepository.Delete(messageToDelete);
            messageRepository.Save();

            return true;
        }
    }
}