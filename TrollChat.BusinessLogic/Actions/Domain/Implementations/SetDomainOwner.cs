using System;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
{
    public class SetDomainOwner : ISetDomainOwner
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUserRepository userRepository;

        public SetDomainOwner(IDomainRepository domainRepository, IUserRepository userRepository)
        {
            this.domainRepository = domainRepository;
            this.userRepository = userRepository;
        }

        public bool Invoke(Guid userId, Guid domainId)
        {
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
            domainRepository.Save();

            return true;
        }
    }
}