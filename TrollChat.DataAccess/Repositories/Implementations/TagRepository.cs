using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(TrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable FindRoomsBy(Expression<Func<Tag, bool>> predicate)
        {
            var query = Include(x => x.Room).Where(predicate).AsQueryable().Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<Tag>().AsQueryable() : query;
        }

        public IQueryable FindUserRoomsBy(Expression<Func<Tag, bool>> predicate)
        {
            var query = context.Set<Tag>().Include(b => b.UserRoom).Where(predicate).Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<Tag>().AsQueryable() : query;
        }
    }
}