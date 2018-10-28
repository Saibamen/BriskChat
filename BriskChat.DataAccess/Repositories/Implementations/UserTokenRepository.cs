using System;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(ITrollChatDbContext context)
           : base(context)
        {
        }

        public override void Add(UserToken entity)
        {
            var timeNow = DateTime.UtcNow;

            entity.ModifiedOn = timeNow;
            entity.CreatedOn = timeNow;
            entity.SecretTokenTimeStamp = timeNow.AddHours(2);
            Context.Set<UserToken>().Add(entity);
        }

        public override void Delete(UserToken entity)
        {
            Context.Set<UserToken>().Remove(entity);
        }
    }
}