using System;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class SetDomainOwner : ISetDomainOwner
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetDomainOwner(IDomainRepository domainRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _domainRepository = domainRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid userId, Guid domainId)
        {
            if (userId == Guid.Empty || domainId == Guid.Empty)
            {
                return false;
            }

            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return false;
            }

            var domain = _domainRepository.GetById(domainId);

            if (domain == null)
            {
                return false;
            }

            domain.Owner = user;

            _domainRepository.Edit(domain);
            _unitOfWork.Save();

            return true;
        }
    }
}