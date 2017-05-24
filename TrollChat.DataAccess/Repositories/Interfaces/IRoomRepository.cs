using System;
using System.Linq;
using System.Runtime.InteropServices;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>, IRepository
    {
        IQueryable<User> GetRoomUsers(Guid roomId);

        IQueryable<Room> GetRoomInformation(Guid roomId);
    }
}