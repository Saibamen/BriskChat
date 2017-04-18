using TrollChat.BusinessLogic.Actions.Domain.Interface;
using TrollChat.DataAccess.Repositories.Interfaces;
using System.Linq;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementation
{
    public class AddNewDomain : IAddNewDomain
    {
        private readonly IDomainRepository domainRepository;

        public AddNewDomain(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public int Invoke(Models.DomainModel domain)
        {
            if (!domain.IsValid() || domainRepository.FindBy(x => x.Name == domain.Name).Any())
            {
                return 0;
            }

            var newDomain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);

            domainRepository.Add(newDomain);
            domainRepository.Save();

            return newDomain.Id;
        }
    }
}