using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class UserDomainRepository : GenericRepository<UserDomain>, IUserDomainRepository
    {
        public UserDomainRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}