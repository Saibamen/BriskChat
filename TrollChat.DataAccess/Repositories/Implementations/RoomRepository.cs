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
            var query = Context.Set<Room>().Include(b => b.Tags).Where(predicate).Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<Room>().AsQueryable() : query;
        }

        public IQueryable<User> GetRoomUsers(Guid roomId)
        {
            var query = from user in Context.Set<User>()
                        join userroom in Context.Set<UserRoom>() on user.Id equals userroom.User.Id
                        join room in Context.Set<Room>() on userroom.Room.Id equals room.Id
                        where room.Id == roomId && user.DeletedOn == null && userroom.DeletedOn == null
                        select new User
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email
                        };

            return !(query.Count() > 0) ? Enumerable.Empty<User>().AsQueryable() : query;
        }

        public IQueryable<Room> GetDomainPublicAndUserRooms(Guid domainId, Guid userId)
        {
            var query = from room in Context.Set<Room>()
                        join userRoom in Context.Set<UserRoom>() on room.Id equals userRoom.Room.Id
                        where room.Domain.Id == domainId && room.DeletedOn == null && (room.IsPublic || userRoom.User.Id == userId && !room.IsPrivateConversation && userRoom.DeletedOn == null)
                        select new Room
                        {
                            Id = room.Id,
                            Name = room.Name,
                            Owner = new User
                            {
                                Name = room.Owner.Name
                            },
                            Description = room.Description,
                            IsPublic = room.IsPublic,
                            CreatedOn = room.CreatedOn
                        };

            return !(query.Count() > 0) ? Enumerable.Empty<Room>().AsQueryable() : query;
        }

        public int GetRoomUsersCount(Guid roomId)
        {
            var query = (from userroom in Context.Set<UserRoom>()
                         join room in Context.Set<Room>() on userroom.Room.Id equals room.Id
                         where room.Id == roomId && userroom.DeletedOn == null
                         select userroom).Count();

            return query;
        }

        public IQueryable<Room> GetRoomInformation(Guid roomId)
        {
            var query = from room in Context.Set<Room>()
                        join user in Context.Set<User>() on room.Owner.Id equals user.Id
                        where room.Id == roomId
                        select new Room
                        {
                            Name = room.Name,
                            Owner = new User
                            {
                                Name = user.Name
                            },
                            Description = room.Description,
                            Topic = room.Topic,
                            Customization = room.Customization,
                            CreatedOn = room.CreatedOn
                        };

            return !(query.Count() > 0) ? Enumerable.Empty<Room>().AsQueryable() : query;
        }
    }
}