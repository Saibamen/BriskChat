using BriskChat.BusinessLogic.Actions.Email.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Email.Implementations
{
    public class AddNewEmailMessage : IAddNewEmailMessage
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewEmailMessage(IEmailRepository emailRepository, IUnitOfWork unitOfWork)
        {
            _emailRepository = emailRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(EmailMessageModel email)
        {
            if (!email.IsValid())
            {
                return false;
            }

            var dbMessage = AutoMapper.Mapper.Map<EmailMessage>(email);

            _emailRepository.Add(dbMessage);
            _unitOfWork.Save();

            return true;
        }
    }
}