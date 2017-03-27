using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class RoomTagRepository : GenericRepository<RoomTag>, IRoomTagRepository
    {
        public RoomTagRepository(TrollChatDbContext context)
           : base(context)
        {
        }
    }
}