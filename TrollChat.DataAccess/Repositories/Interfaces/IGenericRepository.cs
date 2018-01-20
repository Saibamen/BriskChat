using System;
using System.Linq;
using System.Linq.Expressions;

namespace BriskChat.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);

        void Delete(T entity);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        T GetById(Guid id);

        void Edit(T entity);
    }
}
