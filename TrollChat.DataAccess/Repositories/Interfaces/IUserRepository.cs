using System;
using System.Linq;
using System.Linq.Expressions;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>, IRepository
    {
        IQueryable<User> FindTokens(Expression<Func<User, bool>> predicate);
    }
}
