using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class DeleteEmailMessageById : IDeleteEmailMessageById
    {
        private readonly IEmailRepository emailRepository;

        public DeleteEmailMessageById(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public bool Invoke(int emailId)
        {
            var emailToDelete = emailRepository.GetById(emailId);

            if (emailToDelete == null)
            {
                return false;
            }

            emailRepository.Delete(emailToDelete);
            emailRepository.Save();

            return true;
        }
    }
}