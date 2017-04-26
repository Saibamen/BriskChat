using TrollChat.DataAccess.Context;
using TrollChat.BusinessLogic.Configuration.Interfaces;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public class MigrationHelper : IMigrationHelper
    {
        private readonly ITrollChatDbContext dbContext;

        public MigrationHelper(ITrollChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Migrate()
        {
            dbContext.PerformMigration();
        }
    }
}