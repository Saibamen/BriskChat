using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ITrollChatDbContext context)
           : base(context)
        {

        }

        public IQueryable FindRoomsBy(Expression<Func<User, bool>> predicate)
        {
            var query = context.Set<User>().Include(b => b.Rooms).Where(predicate).Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }
    }
}
