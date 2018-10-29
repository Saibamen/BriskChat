using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface ITagRepository : IGenericRepository<Tag>, IRepository
    {
        IQueryable FindRoomsBy(Expression<Func<Tag, bool>> predicate);

        IQueryable FindUserRoomsBy(Expression<Func<Tag, bool>> predicate);
    }
}
