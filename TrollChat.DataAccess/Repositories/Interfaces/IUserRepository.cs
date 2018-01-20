using System;
using System.Linq;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>, IRepository
    {
        IQueryable<Room> GetUserRooms(Guid userId, bool isPrivateConversation);

        IQueryable<UserRoom> GetPrivateConversations(Guid userId);

        IQueryable<UserRoom> GetPrivateConversationsTargets(Guid userId);
    }
}