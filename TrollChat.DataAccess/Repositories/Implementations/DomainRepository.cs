using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class DomainRepository : GenericRepository<Domain>, IDomainRepository
    {
        public DomainRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}