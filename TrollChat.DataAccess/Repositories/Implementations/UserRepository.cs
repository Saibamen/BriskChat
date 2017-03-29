using System;
using System.Linq;
using System.Linq.Expressions;
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

        //TODO: Find use or delete
        public IQueryable FindRoomsBy(Expression<Func<User, bool>> predicate)
        {
            var query = Include(x => x.Rooms).Where(predicate).AsQueryable().Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }

        public IQueryable<User> FindTokens(Expression<Func<User, bool>> predicate)
        {
            var query = Include(x => x.Tokens).Where(predicate).AsQueryable().Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }
    }
}