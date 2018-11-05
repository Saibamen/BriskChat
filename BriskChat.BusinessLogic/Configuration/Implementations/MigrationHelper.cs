using BriskChat.BusinessLogic.Configuration.Interfaces;
using BriskChat.DataAccess.Context;

namespace BriskChat.BusinessLogic.Configuration.Implementations
{
    public class MigrationHelper : IMigrationHelper
    {
        private readonly ITrollChatDbContext _dbContext;

        public MigrationHelper(ITrollChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Migrate()
        {
            _dbContext.PerformMigration();
        }
    }
}