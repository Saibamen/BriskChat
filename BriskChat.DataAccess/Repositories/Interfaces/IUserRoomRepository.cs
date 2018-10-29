using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IUserRoomRepository : IGenericRepository<UserRoom>, IRepository
    {
        IQueryable FindTagsBy(Expression<Func<UserRoom, bool>> predicate);

        IQueryable FindMessagesBy(Expression<Func<UserRoom, bool>> predicate);
    }
}