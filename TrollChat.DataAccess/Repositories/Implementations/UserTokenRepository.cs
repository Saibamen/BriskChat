using System;
using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public override void Add(UserToken entity)
        {
            entity.SecretTokenTimeStamp = DateTime.UtcNow.AddDays(14);
        }
    }
}