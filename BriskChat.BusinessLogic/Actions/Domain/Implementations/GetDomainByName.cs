using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class GetDomainByName : IGetDomainByName
    {
        private readonly IDomainRepository domainRepository;

        public GetDomainByName(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public DomainModel Invoke(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var result = domainRepository.FindBy(x => x.Name == name).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var domainModel = AutoMapper.Mapper.Map<DomainModel>(result);

            return domainModel;
        }
    }
}