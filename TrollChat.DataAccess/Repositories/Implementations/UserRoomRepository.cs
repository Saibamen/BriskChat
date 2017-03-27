using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class UserRoomRepository : GenericRepository<UserRoom>, IUserRoomRepository
    {
        public UserRoomRepository(TrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable FindTagsBy(Expression<Func<UserRoom, bool>> predicate)
        {
            var query = context.Set<UserRoom>().Include(b => b.Tags).Where(predicate).Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<UserRoom>().AsQueryable() : query;
        }

        public IQueryable FindMessagesBy(Expression<Func<UserRoom, bool>> predicate)
        {
            var query = context.Set<UserRoom>().Include(b => b.Messages).Where(predicate).Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<UserRoom>().AsQueryable() : query;
        }
    }
}
