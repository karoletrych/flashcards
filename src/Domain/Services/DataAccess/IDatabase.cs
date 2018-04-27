using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flashcards.Services.DataAccess.Database;

namespace Flashcards.Services.DataAccess
{
	public interface IDatabase
	{
		Task<IEnumerable<T>> GetAllWithChildren<T>(Expression<Func<T, bool>> predicate)
			where T : new();

		Task<T> Single<T>(Expression<Func<T, bool>> predicate) where T : new();

		Task InsertOrReplaceAllWithChildren<T>(IEnumerable<T> entities);
		Task Insert<T>(T entity);

		Task Update<T>(T entity);

		Task Delete<T>(T entity);
		Task DeleteWithChildren<T>(T entity) where T : new();
	}

	public interface IConnection : IDatabase, ITableCreator
	{
	}
}