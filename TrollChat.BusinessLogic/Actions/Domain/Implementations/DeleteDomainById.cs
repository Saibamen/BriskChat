using System;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementations
{
    public class DeleteDomainById : IDeleteDomainById
    {
        private readonly IDomainRepository domainRepository;

        public DeleteDomainById(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public bool Invoke(Guid domainId)
        {
            if (domainId == Guid.Empty)
            {
                return false;
            }

            var domainToDelete = domainRepository.GetById(domainId);

            if (domainToDelete == null)
            {
                return false;
            }

            domainRepository.Delete(domainToDelete);
            domainRepository.Save();

            return true;
        }
    }
}