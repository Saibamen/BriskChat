using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories
{
    public class DomainRoomRepository : GenericRepository<DomainRoom>, IDomainRoomRepository
    {
        public DomainRoomRepository(ITrollChatDbContext context)
           : base(context)
        {
        }
    }
}