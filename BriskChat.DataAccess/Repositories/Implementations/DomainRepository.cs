using System;
using System.Linq;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class DomainRepository : GenericRepository<Domain>, IDomainRepository
    {
        public DomainRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public Domain GetDomainByUserId(Guid userGuid)
        {
            var query = Context.Set<User>()
                .Include(x => x.Domain)
                .FirstOrDefault(x => x.Id == userGuid)
                .Domain;

            return query;
        }
    }
}