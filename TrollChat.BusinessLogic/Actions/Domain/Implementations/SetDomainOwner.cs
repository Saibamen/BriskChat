using System;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
{
    public class SetDomainOwner : ISetDomainOwner
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetDomainOwner(IDomainRepository domainRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.domainRepository = domainRepository;
            this.userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid userId, Guid domainId)
        {
            if (userId == Guid.Empty || domainId == Guid.Empty)
            {
                return false;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return false;
            }

            var domain = domainRepository.GetById(domainId);

            if (domain == null)
            {
                return false;
            }

            domain.Owner = user;

            domainRepository.Edit(domain);
            _unitOfWork.Save();

            return true;
        }
    }
}