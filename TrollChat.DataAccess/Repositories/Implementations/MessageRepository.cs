using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable<Message> GetLastRoomMessages(Guid roomId, int number)
        {
            var query = context.Set<Message>().Include(x => x.UserRoom.User)
                .Where(x => x.UserRoom.Room.Id == roomId).OrderByDescending(x => x.CreatedOn).Take(number);

            return query;
        }
    }
}