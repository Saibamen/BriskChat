using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class DomainRepository : GenericRepository<Domain>, IDomainRepository
    {
        public DomainRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public Domain GetDomainByUserId(Guid userGuid)
        {
            var query = Context.Set<User>().Include(x => x.Domain).FirstOrDefault(x => x.Id == userGuid).Domain;

            return query;
        }
    }
}