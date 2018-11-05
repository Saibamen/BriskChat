using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class CheckDomainExistsByName : ICheckDomainExistsByName
    {
        private readonly IDomainRepository _domainRepository;

        public CheckDomainExistsByName(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public bool Invoke(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var result = _domainRepository.FindBy(x => x.Name == name);

            return result.Count() > 0;
        }
    }
}