using System;
using System.Linq;
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
            var query = (from room in context.Set<Room>()
                         join userroom in context.Set<UserRoom>() on room.Id equals userroom.Room.Id
                         join message in context.Set<Message>() on userroom.Id equals message.UserRoom.Id
                         where room.Id == roomId && message.DeletedOn == null
                         select new Message
                         {
                             Id = message.Id,
                             Text = message.Text,
                             CreatedOn = message.CreatedOn,
                             UserRoom = new UserRoom
                             {
                                 User = new User { Name = message.UserRoom.User.Name }
                             }
                         }).OrderByDescending(x => x.CreatedOn).Take(number);

            return query;
        }
    }
}