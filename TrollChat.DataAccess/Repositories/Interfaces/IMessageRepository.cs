using System;
using System.Linq;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>, IRepository
    {
        IQueryable<Message> GetLastRoomMessages(Guid roomId, int number);

        IQueryable<Message> GetRoomMessagesOffset(Guid roomId, DateTime lastMessageDate, int limit);
    }
}