using System;
using System.Linq;
using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>, IRepository
    {
        IQueryable<Message> GetLastRoomMessages(Guid roomId, int number);

        IQueryable<Message> GetRoomMessagesOffset(Guid roomId, DateTime lastMessageDate, int limit);
    }
}