using System;
using BriskChat.BusinessLogic.Actions.UserDomain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserDomain.Implementations
{
    public class AddUserToDomain : IAddUserToDomain
    {
        private readonly IUserDomainRepository _userDomainRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDomainRepository _domainRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToDomain(IUserDomainRepository userDomainRepository,
            IUserRepository userRepository,
            IDomainRepository domainRepository,
            IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _userDomainRepository = userDomainRepository;
            _userRepository = userRepository;
            _domainRepository = domainRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid userId, Guid domainId, Guid roleId)
        {
            if (userId == Guid.Empty || domainId == Guid.Empty || roleId == Guid.Empty)
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

            var role = _roleRepository.GetById(roleId);

            if (role == null)
            {
                return false;
            }

            var userDomain = new DataAccess.Models.UserDomain
            {
                User = user,
                Domain = domain,
                Role = role
            };

            _userDomainRepository.Add(userDomain);
            _unitOfWork.Save();

            return true;
        }
    }
}