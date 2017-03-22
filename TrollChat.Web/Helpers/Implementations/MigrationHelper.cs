using TrollChat.DataAccess.Context;
using TrollChat.Web.Helpers.Interfaces;

namespace TrollChat.Web.Helpers.Implementations
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
