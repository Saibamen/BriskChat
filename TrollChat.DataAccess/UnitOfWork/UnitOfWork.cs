using TrollChat.DataAccess.Context;

namespace TrollChat.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ITrollChatDbContext _dbContext;

        public UnitOfWork(ITrollChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}