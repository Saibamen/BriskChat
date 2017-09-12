using System.Linq;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
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
            if (string.IsNullOrEmpty(name))
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