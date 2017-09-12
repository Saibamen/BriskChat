using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IEmailRepository : IGenericRepository<EmailMessage>, IRepository
    {
    }
}