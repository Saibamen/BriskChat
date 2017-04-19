using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Domain.Interface;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.Domain.Implementation
{
    public class DeleteDomainById : IDeleteDomainById
    {
        private readonly IDomainRepository domainRepository;

        public DeleteDomainById(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public bool Invoke(int domainId)
        {
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