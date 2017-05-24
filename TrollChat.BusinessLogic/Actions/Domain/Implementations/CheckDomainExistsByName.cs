using System.Linq;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
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