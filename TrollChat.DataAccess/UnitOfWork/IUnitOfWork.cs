using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IRepository
    {
        void Save();
    }
}