using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>, IRepository
    {
        IQueryable FindTagsBy(Expression<Func<Room, bool>> predicate);

        IQueryable<User> GetRoomUsers(Guid roomId);

        IQueryable<Room> GetDomainPublicAndUserRooms(Guid domainId, Guid userId);

        int GetRoomUsersCount(Guid roomId);

        IQueryable<Room> GetRoomInformation(Guid roomId);
    }
}