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
            var query = (from message in context.Set<Message>()
                         where message.DeletedOn == null && message.UserRoom.Room.Id == roomId
                         orderby message.CreatedOn descending
                         select new Message
                         {
                             Id = message.Id,
                             Text = message.Text,
                             CreatedOn = message.CreatedOn,
                             UserRoom = new UserRoom
                             {
                                 User = new User
                                 {
                                     Id = message.UserRoom.User.Id,
                                     Name = message.UserRoom.User.Name
                                 }
                             }
                         }).Take(number);

            return !(query.Count() > 0) ? Enumerable.Empty<Message>().AsQueryable() : query;
        }

        public IQueryable<Message> GetRoomMessagesOffset(Guid roomId, int loadedMessagesIteration, int limit)
        {
            var query = (from message in context.Set<Message>()
                         where message.DeletedOn == null && message.UserRoom.Room.Id == roomId
                         orderby message.CreatedOn descending
                         select new Message
                         {
                             Id = message.Id,
                             Text = message.Text,
                             CreatedOn = message.CreatedOn,
                             UserRoom = new UserRoom
                             {
                                 User = new User
                                 {
                                     Id = message.UserRoom.User.Id,
                                     Name = message.UserRoom.User.Name
                                 }
                             }
                         }).Skip(limit * loadedMessagesIteration).Take(limit);

            return !(query.Count() > 0) ? Enumerable.Empty<Message>().AsQueryable() : query;
        }
    }
}