using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.DataAccess.Context
{
    public interface ITrollChatDbContext : IRepository
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        void SetModified(object entity);

        void PerformMigration();

    }
}