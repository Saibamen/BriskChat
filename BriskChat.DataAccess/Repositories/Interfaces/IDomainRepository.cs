using System;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IDomainRepository : IGenericRepository<Domain>, IRepository
    {
        Domain GetDomainByUserId(Guid userGuid);
    }
}