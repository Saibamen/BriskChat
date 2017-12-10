using System;
using BriskChat.BusinessLogic.Actions.UserDomain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserDomain.Implementations
{
    public class AddUserToDomain : IAddUserToDomain
    {
        private readonly IUserDomainRepository userDomainRepository;
        private readonly IUserRepository userRepository;
        private readonly IDomainRepository domainRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToDomain(IUserDomainRepository userDomainRepository,
            IUserRepository userRepository,
            IDomainRepository domainRepository,
            IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            this.userDomainRepository = userDomainRepository;
            this.userRepository = userRepository;
            this.domainRepository = domainRepository;
            this.roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid userId, Guid domainId, Guid roleId)
        {
            if (userId == Guid.Empty || domainId == Guid.Empty || roleId == Guid.Empty)
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

            var role = roleRepository.GetById(roleId);

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

            userDomainRepository.Add(userDomain);
            _unitOfWork.Save();

            return true;
        }
    }
}