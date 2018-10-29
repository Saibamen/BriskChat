using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class UserRoomRepository : GenericRepository<UserRoom>, IUserRoomRepository
    {
        public UserRoomRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable FindTagsBy(Expression<Func<UserRoom, bool>> predicate)
        {
            var query = Context.Set<UserRoom>()
                .Include(b => b.Tags)
                .Where(predicate)
                .Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<UserRoom>().AsQueryable() : query;
        }

        public IQueryable FindMessagesBy(Expression<Func<UserRoom, bool>> predicate)
        {
            var query = Context.Set<UserRoom>()
                .Include(b => b.Messages)
                .Where(predicate)
                .Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<UserRoom>().AsQueryable() : query;
        }
    }
}