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

        public IQueryable<User> GetUserAndRoomsByUserId(int id)
        {
            var query = Include(x => x.Rooms).AsQueryable().Where(x => x.DeletedOn == null).Where(y => y.Id == id);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }

        public IQueryable<User> GetUserAndUserRoomsByUserId(int id)
        {
            var query = Include(x => x.UserRooms).Include(i => i.UserRooms).ThenInclude(i => i.Room).AsQueryable().Where(x => x.DeletedOn == null).Where(y => y.Id == id);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }

        public IQueryable<User> FindTokens(Expression<Func<User, bool>> predicate)
        {
            var query = Include(x => x.Tokens).Where(predicate).AsQueryable().Where(x => x.DeletedOn == null);
            return !query.Any() ? Enumerable.Empty<User>().AsQueryable() : query;
        }
    }
}