using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess
{
    public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate);

        Task<int> Insert(T entity);
        Task Update(T entity);
        event EventHandler<T> ObjectInserted;
        Task Delete(T entity);
        Task UpdateAll(IEnumerable<T> entities);
    }
}