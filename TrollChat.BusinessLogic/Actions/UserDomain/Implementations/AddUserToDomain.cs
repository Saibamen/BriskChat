using System;
using TrollChat.BusinessLogic.Actions.UserDomain.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserDomain.Implementations
{
    public class AddUserToDomain : IAddUserToDomain
    {
        private readonly IUserDomainRepository userDomainRepository;
        private readonly IUserRepository userRepository;
        private readonly IDomainRepository domainRepository;
        private readonly IRoleRepository roleRepository;

        public AddUserToDomain(IUserDomainRepository userDomainRepository,
            IUserRepository userRepository,
            IDomainRepository domainRepository,
            IRoleRepository roleRepository)
        {
            this.userDomainRepository = userDomainRepository;
            this.userRepository = userRepository;
            this.domainRepository = domainRepository;
            this.roleRepository = roleRepository;
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
            userDomainRepository.Save();

            return true;
        }
    }
}