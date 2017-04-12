using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class DeleteEmailMessage : IDeleteEmailMessage
    {
        private readonly IEmailRepository emailRepository;

        public DeleteEmailMessage(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public void Invoke(EmailMessage email)
        {
            emailRepository.Delete(email);
        }
    }
}