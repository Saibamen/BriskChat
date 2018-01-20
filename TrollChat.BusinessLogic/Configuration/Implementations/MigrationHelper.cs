using BriskChat.BusinessLogic.Configuration.Interfaces;
using BriskChat.DataAccess.Context;

namespace BriskChat.BusinessLogic.Configuration.Implementations
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