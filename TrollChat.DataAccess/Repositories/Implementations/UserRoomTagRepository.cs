using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class UserRoomTagRepository : GenericRepository<UserRoomTag>, IUserRoomTagRepository
    {
        public UserRoomTagRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}
