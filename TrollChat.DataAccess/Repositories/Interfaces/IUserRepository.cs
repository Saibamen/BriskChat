using System;
using System.Linq;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>, IRepository
    {
        IQueryable<Room> GetUserRooms(Guid id, bool isPrivateConversation);
    }
}