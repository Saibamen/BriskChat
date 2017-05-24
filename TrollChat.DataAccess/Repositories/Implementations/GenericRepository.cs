using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public IQueryable<T> DbSet
        {
            get { return dbSet.Where(e => e.DeletedOn == null); }
        }

        private readonly DbSet<T> dbSet;

        protected ITrollChatDbContext context;

        protected GenericRepository(ITrollChatDbContext _context)
        {
            context = _context;

            dbSet = _context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            var timeNow = DateTime.UtcNow;
            entity.Id = new Guid();
            entity.ModifiedOn = timeNow;
            entity.CreatedOn = timeNow;
            dbSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            entity.DeletedOn = DateTime.UtcNow;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = context.Set<T>().Where(predicate).Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<T>().AsQueryable() : query;
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            DbSet<T> dbSet = context.Set<T>();

            IQueryable<T> query = null;

            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return query ?? dbSet;
        }

        public virtual T GetById(Guid id)
        {
            return DbSet.FirstOrDefault(m => m.Id == id);
        }

        public virtual void Edit(T entity)
        {
            context.SetModified(entity);
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }
    }
}