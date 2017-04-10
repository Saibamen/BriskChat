using System.Collections.Generic;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class GetEmailLog : IGetEmailLog
    {
        private readonly IEmailRepository emailRepository;

        public GetEmailLog(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        public List<EmailLogger> Invoke()
        {
            return emailRepository.FindBy(x => x.Message != null).ToList();
        }
    }
}