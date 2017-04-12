using System.Collections.Generic;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class GetEmailMessages : IGetEmailMessages
    {
        private readonly IEmailRepository emailRepository;

        public GetEmailMessages(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public List<EmailMessage> Invoke()
        {
            return emailRepository.FindBy(x => x.Message != null).Take(10).ToList();
        }
    }
}