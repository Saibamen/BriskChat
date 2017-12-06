using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class RoomTagRepository : GenericRepository<RoomTag>, IRoomTagRepository
    {
        public RoomTagRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}