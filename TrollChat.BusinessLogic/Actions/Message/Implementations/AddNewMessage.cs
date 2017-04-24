using TrollChat.BusinessLogic.Actions.Message.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Message.Implementations
{
    public class AddNewMessage : IAddNewMessage
    {
        private readonly IMessageRepository messageRepository;

        public AddNewMessage(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public bool Invoke(MessageModel message)
        {
            if (!message.IsValid())
            {
                return false;
            }

            var dbMessage = AutoMapper.Mapper.Map<DataAccess.Models.Message>(message);

            messageRepository.Add(dbMessage);
            messageRepository.Save();

            return true;
        }
    }
}