using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class CheckDomainExistsByName : ICheckDomainExistsByName
    {
        private readonly IDomainRepository domainRepository;

        public CheckDomainExistsByName(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public bool Invoke(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var result = domainRepository.FindBy(x => x.Name == name);

            return result.Count() > 0;
        }
    }
}