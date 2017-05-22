using System;
using System.Linq;
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

        public IQueryable<Room> GetUserRooms(Guid userId, bool isPrivateConversation)
        {
            var query =
                context.Set<User>()
                .Where(user => user.DeletedOn == null)
                .Where(user => user.Id == userId)
                    .SelectMany(i => i.UserRooms)
                    .Where(userRoom => userRoom.DeletedOn == null)
                         .Select(i => i.Room)
                         .Where(room => room.IsPrivateConversation == isPrivateConversation)
                         .Where(room => room.DeletedOn == null)
                .AsQueryable();

            return query.Any() ? query : Enumerable.Empty<Room>().AsQueryable();
        }

        public IQueryable<UserRoom> GetPrivateConversations(Guid userId)
        {
            var query = from user in context.Set<User>()
                        join useroom in context.Set<UserRoom>() on user.Id equals useroom.User.Id
                        join room in context.Set<Room>() on useroom.Room.Id equals room.Id
                        where user.Id == userId
                        where room.IsPrivateConversation
                        select new UserRoom
                        {
                            Id = useroom.Id,
                            User = user,
                            Room = room
                        };

            return query.Any() ? query : Enumerable.Empty<UserRoom>().AsQueryable();
        }

        public IQueryable<UserRoom> GetPrivateConversationsTargets(Guid userId)
        {
            var query1 = (from user in context.Set<User>()
                          join useroom in context.Set<UserRoom>() on user.Id equals useroom.User.Id
                          join room in context.Set<Room>() on useroom.Room.Id equals room.Id
                          where user.Id == userId
                          where room.IsPrivateConversation
                          select room).ToList();

            var query2 = from room in context.Set<Room>()
                         join useroom in context.Set<UserRoom>() on room.Id equals useroom.Room.Id
                         join user in context.Set<User>() on useroom.User.Id equals user.Id
                         where query1.Contains(room)
                         where useroom.User.Id != userId
                         select new UserRoom
                         {
                             Id = useroom.Id,
                             User = user,
                             Room = room
                         };

            return query2.Any() ? query2 : Enumerable.Empty<UserRoom>().AsQueryable();
        }
    }
}