using TrollChat.DataAccess.Context;
using TrollChat.BusinessLogic.Configuration.Interfaces;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public class MigrationHelper : IMigrationHelper
    {
        private readonly ITrollChatDbContext DbContext;
        public MigrationHelper(ITrollChatDbContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public void Migrate()
        {
            DbContext.PerformMigration();
        }
    }
}
