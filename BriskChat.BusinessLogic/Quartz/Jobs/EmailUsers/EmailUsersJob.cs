using System.Diagnostics;
using System.Threading.Tasks;
using BriskChat.BusinessLogic.Actions.Email.Interfaces;
using BriskChat.BusinessLogic.Helpers.Extensions;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using Quartz;

namespace BriskChat.BusinessLogic.Quartz.Jobs.EmailUsers
{
    [DisallowConcurrentExecution]
    public class EmailUsersJob : IJob
    {
        private readonly IEmailService _emailService;
        private readonly IGetEmailMessages _getEmailMessages;
        private readonly IDeleteEmailMessageById _deleteEmailMessageById;

        public EmailUsersJob(IGetEmailMessages getEmailMessages,
            IEmailService emailService,
            IDeleteEmailMessageById deleteEmailMessageById)
        {
            _emailService = emailService;
            _getEmailMessages = getEmailMessages;
            _deleteEmailMessageById = deleteEmailMessageById;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var emailList = _getEmailMessages.Invoke(10);

            if (!(emailList.Count > 0))
            {
                Debug.WriteLine("No emails to send");
                return;
            }

            var connectionResult = await _emailService.ConnectClient();

            if (!connectionResult)
            {
                Debug.WriteLine("Authentication error");
                return;
            }

            await emailList.ForEachAsync(async email =>
            {
                var messageToSend = _emailService.CreateMessage(email.Recipient, email.Subject, email.Message);
                var sendResult = await _emailService.SendEmailAsync(messageToSend);

                if (sendResult)
                {
                    _deleteEmailMessageById.Invoke(email.Id);
                }
                else
                {
                    Debug.WriteLine("Email couldn't be sent");
                }
            });

            await _emailService.DisconnectClient();
        }
    }
}