using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess
{
	public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> GetAllWithChildren(Expression<Func<T,bool>> predicate, bool recursive);

	    Task<T> Single(Expression<Func<T, bool>> predicate);

	    Task Insert(T entity);
		Task InsertOrReplaceAllWithChildren(IEnumerable<T> entities);

	    Task Update(T entity);

        Task Delete(T entity);
	    Task DeleteWithChildren(T enitity);
    }
}