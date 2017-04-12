using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class AddNewEmailMessage : IAddNewEmailMessage
    {
        private readonly IEmailRepository emailRepository;

        public AddNewEmailMessage(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public bool Invoke(EmailMessageModel email)
        {
            if (!email.IsValid())
            {
                return false;
            }

            var dbMessage = AutoMapper.Mapper.Map<EmailMessage>(email);

            emailRepository.Add(dbMessage);
            emailRepository.Save();

            return true;
        }
    }
}