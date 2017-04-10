using System.Linq;
using MimeKit;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class AddNewEmailLog : IAddNewEmailLog
    {
        private readonly IEmailRepository emailRepository;

        public AddNewEmailLog(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public void Invoke(MimeMessage email)
        {
            var dbItem = new EmailLogger()
            {
                From = email.From.Mailboxes.FirstOrDefault().Address,
                Recipient = email.To.Mailboxes.FirstOrDefault().Address,
                Message = email.HtmlBody,
                Subject = email.Subject,
                FailureCount = 0
            };

            emailRepository.Add(dbItem);
            emailRepository.Save();
        }
    }
}