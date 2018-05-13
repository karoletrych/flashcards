using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flashcards.Services.DataAccess;

namespace Flashcards.Infrastructure.DataAccess
{
	public class Repository<T> : IRepository<T>, INotifyObjectInserted<T> where T : new()
	{
		private readonly Func<IDatabase> _dbConnection;

		public Repository(Func<IDatabase> dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public Task<IEnumerable<T>> GetWithChildren(Expression<Func<T,bool>> predicate)
		{
			return _dbConnection().GetAllWithChildren(predicate);
		}

		public Task<IEnumerable<T>> GetAllWithChildren()
		{
			return _dbConnection().GetAllWithChildren<T>(null);
		}

		public Task<T> Single(Expression<Func<T, bool>> predicate)
		{
			return _dbConnection().Single(predicate);
		}

		public Task<T> SingleWithChildren(Expression<Func<T, bool>> predicate)
		{
			return _dbConnection().SingleWithChildren(predicate);
		}

		public event EventHandler<T> ObjectInserted;

		public async Task Insert(T entity)
		{
			await _dbConnection().Insert(entity);

			ObjectInserted?.Invoke(this, entity);
		}

		public Task Update(T entity)
		{
			return _dbConnection().Update(entity);
		}

		public Task InsertOrReplaceAllWithChildren(IEnumerable<T> entities)
		{
			return _dbConnection().InsertOrReplaceAllWithChildren(entities);
		}

		public Task Delete(T entity)
		{
			return _dbConnection().Delete(entity);
		}

		public Task DeleteWithChildren(T entity)
		{
			return _dbConnection().DeleteWithChildren(entity);
		}

		public Task<bool> Any()
		{
			return _dbConnection().Any<T>();
		}
	}
}