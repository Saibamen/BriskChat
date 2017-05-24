using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
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

            return !(query.Count() > 0) ? Enumerable.Empty<Room>().AsQueryable() : query;
        }

        public IQueryable<User> GetRoomUsers(Guid roomId)
        {
            var query = from u in context.Set<User>()
                         join userroom in context.Set<UserRoom>() on user.Id equals userroom.User.Id
                         join room in context.Set<Room>() on userroom.Room.Id equals room.Id
                         where room.Id == roomId
                         select new User
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Email = user.Email
                         };

            return !(query.Count() > 0) ? Enumerable.Empty<User>().AsQueryable() : query;
        }

        public IQueryable<Room> GetRoomInformation(Guid roomId)
        {
            var query1 = from room in context.Set<Room>()
                         join user in context.Set<User>() on room.Owner.Id equals user.Id
                         where room.Id == roomId
                         select new Room
                         {
                             Name = room.Name,
                             Owner = user,
                             Description = room.Description,
                             Topic = room.Topic,
                             Customization = room.Customization,
                             CreatedOn = room.CreatedOn,
                         };

            return !query1.Any() ? Enumerable.Empty<Room>().AsQueryable() : query1;
        }
    }
}