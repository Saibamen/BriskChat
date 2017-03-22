using System;
using System.Linq;
using System.Linq.Expressions;

namespace TrollChat.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);

        void Delete(T entity);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        T GetById(int id);

        void Save();

        void Edit(T entity);
    }
}
