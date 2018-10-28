using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}