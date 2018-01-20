using System;
using System.Linq;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public IQueryable<Message> GetLastRoomMessages(Guid roomId, int number)
        {
            var query = (from message in Context.Set<Message>()
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
                                     Name = message.UserRoom.User.Name,
                                     Email = message.UserRoom.User.Email
                                 }
                             }
                         }).Take(number);

            return !(query.Count() > 0) ? Enumerable.Empty<Message>().AsQueryable() : query;
        }

        public IQueryable<Message> GetRoomMessagesOffset(Guid roomId, DateTime lastMessageDate, int limit)
        {
            var query = (from message in Context.Set<Message>()
                         where message.DeletedOn == null && message.UserRoom.Room.Id == roomId && message.CreatedOn < lastMessageDate
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
                                     Name = message.UserRoom.User.Name,
                                     Email = message.UserRoom.User.Email
                                 }
                             }
                         }).Take(limit);

            return !(query.Count() > 0) ? Enumerable.Empty<Message>().AsQueryable() : query;
        }
    }
}