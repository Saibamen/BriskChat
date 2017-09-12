using System;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IDomainRepository : IGenericRepository<Domain>, IRepository
    {
        Domain GetDomainByUserId(Guid userGuid);
    }
}