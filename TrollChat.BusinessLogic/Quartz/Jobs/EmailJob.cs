using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using Quartz;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;

namespace TrollChat.BusinessLogic.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public class EmailJob : IJob
    {
        private readonly IEmailService emailService;
        private readonly IGetEmailLog getEmailLog;

        public EmailJob(IGetEmailLog getEmailLog, IEmailService emailService)
        {
            this.emailService = emailService;
            this.getEmailLog = getEmailLog;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var emailList = getEmailLog.Invoke();
            if (!emailList.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var email in emailList)
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("TrollChat", email.From));
                mimeMessage.To.Add(new MailboxAddress("", email.Recipient));
                mimeMessage.Subject = email.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = email.Message
                };
                mimeMessage.Body = builder.ToMessageBody();

                Debug.WriteLine("Email sent to user");

                emailService.SendEmailAsync(mimeMessage);
            }
            return Task.CompletedTask;
        }
    }
}