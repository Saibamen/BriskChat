using System;
using System.Collections.Generic;
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

        public IQueryable<Room> GetUserRooms(Guid id, bool isPrivateConversation)
        {
            var query =
                context.Set<User>()
                .Where(user => user.DeletedOn == null)
                .Where(user => user.Id == id)
                    .SelectMany(i => i.UserRooms)
                    .Where(userRoom => userRoom.DeletedOn == null)
                         .Select(i => i.Room)
                         .Where(room => room.IsPrivateConversation == isPrivateConversation)
                .AsQueryable();

            return query.Any() ? query : Enumerable.Empty<Room>().AsQueryable();
        }
    }
}