using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IRepository
    {
        void Save();
    }
}