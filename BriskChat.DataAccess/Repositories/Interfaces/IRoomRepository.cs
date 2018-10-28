using System;
using System.Linq;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>, IRepository
    {
        IQueryable<User> GetRoomUsers(Guid roomId);

        IQueryable<Room> GetDomainPublicAndUserRooms(Guid domainId, Guid userId);

        int GetRoomUsersCount(Guid roomId);

        IQueryable<Room> GetRoomInformation(Guid roomId);
    }
}