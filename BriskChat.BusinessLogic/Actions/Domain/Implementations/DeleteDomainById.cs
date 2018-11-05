using System;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class DeleteDomainById : IDeleteDomainById
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDomainById(IDomainRepository domainRepository, IUnitOfWork unitOfWork)
        {
            _domainRepository = domainRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid domainId)
        {
            if (domainId == Guid.Empty)
            {
                return false;
            }

            var domainToDelete = _domainRepository.GetById(domainId);

            if (domainToDelete == null)
            {
                return false;
            }

            _domainRepository.Delete(domainToDelete);
            _unitOfWork.Save();

            return true;
        }
    }
}