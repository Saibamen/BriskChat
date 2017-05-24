using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Helpers.Extensions;
using TrollChat.BusinessLogic.Helpers.Interfaces;

namespace TrollChat.BusinessLogic.Quartz.Jobs.EmailUsers
{
    [DisallowConcurrentExecution]
    public class EmailUsersJob : IJob
    {
        private readonly IEmailService emailService;
        private readonly IGetEmailMessages getEmailMessages;
        private readonly IDeleteEmailMessageById deleteEmailMessageById;

        public EmailUsersJob(IGetEmailMessages getEmailMessages,
            IEmailService emailService,
            IDeleteEmailMessageById deleteEmailMessageById)
        {
            this.emailService = emailService;
            this.getEmailMessages = getEmailMessages;
            this.deleteEmailMessageById = deleteEmailMessageById;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var emailList = getEmailMessages.Invoke(10);

            if (!(emailList.Count > 0))
            {
                Debug.WriteLine("No emails to send");
                return;
            }

            var connectionResult = await emailService.ConnectClient();

            if (!connectionResult)
            {
                Debug.WriteLine("Authentication error");
                return;
            }

            await emailList.ForEachAsync(async email =>
            {
                var messageToSend = emailService.CreateMessage(email.Recipient, email.Subject, email.Message);
                var sendResult = await emailService.SendEmailAsync(messageToSend);

                if (sendResult)
                {
                    deleteEmailMessageById.Invoke(email.Id);
                }
                else
                {
                    Debug.WriteLine("Email couldn't be sent");
                }
            });

            await emailService.DisconnectClient();
        }
    }
}