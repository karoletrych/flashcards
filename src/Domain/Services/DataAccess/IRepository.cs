using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess
{
	// TODO: think about splitting into separete repositories for each aggregate root
	public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> GetWithChildren(Expression<Func<T,bool>> predicate);
        Task<IEnumerable<T>> GetAllWithChildren();

	    Task<T> Single(Expression<Func<T, bool>> predicate);

	    Task Insert(T entity);
		Task InsertOrReplaceAllWithChildren(IEnumerable<T> entities);

	    Task Update(T entity);

        Task Delete(T entity);
	    Task DeleteWithChildren(T enitity);
    }
}