using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>, IRepository
    {
    }
}