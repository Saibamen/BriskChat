using BriskChat.DataAccess.Models;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IEmailRepository : IGenericRepository<EmailMessage>, IRepository
    {
    }
}