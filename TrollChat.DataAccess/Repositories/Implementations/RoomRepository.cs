using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ITrollChatDbContext context) : base(context)
        {
        }

        public IQueryable FindTagsBy(Expression<Func<Room, bool>> predicate)
        {
            var query = context.Set<Room>().Include(b => b.Tags).Where(predicate).Where(x => x.DeletedOn == null);

            return !query.Any() ? Enumerable.Empty<Room>().AsQueryable() : query;
        }
    }
}