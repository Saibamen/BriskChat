using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Email.Implementations
{
    public class AddNewEmailMessage : IAddNewEmailMessage
    {
        private readonly IEmailRepository emailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewEmailMessage(IEmailRepository emailRepository, IUnitOfWork unitOfWork)
        {
            this.emailRepository = emailRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(EmailMessageModel email)
        {
            if (!email.IsValid())
            {
                return false;
            }

            var dbMessage = AutoMapper.Mapper.Map<EmailMessage>(email);

            emailRepository.Add(dbMessage);
            _unitOfWork.Save();

            return true;
        }
    }
}