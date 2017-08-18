using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class UserDomainRepository : GenericRepository<UserDomain>, IUserDomainRepository
    {
        public UserDomainRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}