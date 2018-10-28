using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class AddNewDomain : IAddNewDomain
    {
        private readonly IDomainRepository domainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewDomain(IDomainRepository domainRepository, IUnitOfWork unitOfWork)
        {
            this.domainRepository = domainRepository;
            _unitOfWork = unitOfWork;
        }

        public Guid Invoke(DomainModel domain, Guid? userId = null)
        {
            if (!domain.IsValid() || domainRepository.FindBy(x => x.Name == domain.Name).Count() > 0)
            {
                return Guid.Empty;
            }

            var newDomain = AutoMapper.Mapper.Map<DataAccess.Models.Domain>(domain);
            domainRepository.Add(newDomain);
            _unitOfWork.Save();

            return newDomain.Id;
        }
    }
}