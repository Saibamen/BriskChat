using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class UserRoomTagRepository : GenericRepository<UserRoomTag>, IUserRoomTagRepository
    {
        public UserRoomTagRepository(TrollChatDbContext context)
           : base(context)
        {
        }
    }
}
