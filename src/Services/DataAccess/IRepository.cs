using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess
{
    public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindWhere(Expression<Func<T, bool>> predicate);

	    event EventHandler<T> ObjectInserted;
		Task Insert(T entity);

        Task InsertOrReplaceAll(IEnumerable<T> entities);
        Task Update(T entity);

        Task Delete(T entity);
    }
}