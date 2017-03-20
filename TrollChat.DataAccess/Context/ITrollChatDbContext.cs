using Microsoft.EntityFrameworkCore;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Context
{
    public interface ITrollChatDbContext : IRepository
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        void SetModified(object entity);        

    }
}