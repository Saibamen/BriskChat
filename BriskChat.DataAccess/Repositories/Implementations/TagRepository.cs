using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable FindRoomsBy(Expression<Func<Tag, bool>> predicate)
        {
            var query = Include(x => x.Room)
                .Where(predicate)
                .Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<Tag>().AsQueryable() : query;
        }

        public IQueryable FindUserRoomsBy(Expression<Func<Tag, bool>> predicate)
        {
            var query = Context.Set<Tag>()
                .Include(b => b.UserRoom)
                .Where(predicate)
                .Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<Tag>().AsQueryable() : query;
        }
    }
}