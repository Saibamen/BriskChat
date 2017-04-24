using System;
using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class DeleteMessageById : IDeleteMessageById
    {
        private readonly IMessageRepository MessageRepository;

        public DeleteMessageById(IMessageRepository MessageRepository)
        {
            this.MessageRepository = MessageRepository;
        }

        public bool Invoke(Guid MessageId)
        {
            var messageToDelete = MessageRepository.GetById(MessageId);

            if (messageToDelete == null)
            {
                return false;
            }

            MessageRepository.Delete(messageToDelete);
            MessageRepository.Save();

            return true;
        }
    }
}