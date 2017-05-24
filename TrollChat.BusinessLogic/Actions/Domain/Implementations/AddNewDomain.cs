using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
{
    public class AddNewDomain : IAddNewDomain
    {
        private readonly IDomainRepository domainRepository;

        public AddNewDomain(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public Guid Invoke(DomainModel domain, Guid? userId = null)
        {
            if (!domain.IsValid() || domainRepository.FindBy(x => x.Name == domain.Name).Count() > 0)
            {
                return Guid.Empty;
            }

            var newDomain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);
            domainRepository.Add(newDomain);
            domainRepository.Save();

            return newDomain.Id;
        }
    }
}