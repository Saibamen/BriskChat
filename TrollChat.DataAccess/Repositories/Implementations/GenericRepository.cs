using System;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.DataAccess.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public IQueryable<T> DbSet
        {
            get { return dbSet.Where(e => e.DeletedOn == null); }
        }

        private readonly DbSet<T> dbSet;
        protected ITrollChatDbContext Context;

        protected GenericRepository(ITrollChatDbContext context)
        {
            Context = context;

            dbSet = context.Set<T>();
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
            IQueryable<T> query = Context.Set<T>().Where(predicate).Where(x => x.DeletedOn == null);

            return !(query.Count() > 0) ? Enumerable.Empty<T>().AsQueryable() : query;
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            DbSet<T> dbSet = Context.Set<T>();

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
            Context.SetModified(entity);
        }
    }
}