using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Models;

namespace TrollChat.DataAccess.Context
{
    public class TrollChatDbContext : DbContext, ITrollChatDbContext
    {
        public TrollChatDbContext()
        {
        }

        public TrollChatDbContext(DbContextOptions<TrollChatDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=TrollChat;Integrated Security=True;");
            }
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void PerformMigration()
        {
            Database.Migrate();
        }

        #region DbSet

        public DbSet<User> Users { get; set; }

        #endregion DbSet
    }
}