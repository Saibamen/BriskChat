using System;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Domain.Implementations
{
    public class GetDomainByUserId : IGetDomainByUserId
    {
        private readonly IDomainRepository _domainRepository;

        public GetDomainByUserId(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public DomainModel Invoke(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
            {
                return null;
            }

            var result = _domainRepository
                .FindBy(x => x.Users.Any(y => y.Id == userGuid))
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var domainModel = AutoMapper.Mapper.Map<DomainModel>(result);

            return domainModel;
        }
    }
}