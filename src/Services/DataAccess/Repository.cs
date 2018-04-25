using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flashcards.Services.DataAccess.Database;

namespace Flashcards.Services.DataAccess
{
	public class Repository<T> : IRepository<T>, INotifyObjectInserted<T> where T : new()
	{
		private readonly Func<IDatabase> _dbConnection;

		public Repository(Func<IDatabase> dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public Task<IEnumerable<T>> GetAllWithChildren(Expression<Func<T,bool>> predicate, bool recursive)
		{
			return _dbConnection()
				.GetAllWithChildren(predicate, recursive: recursive);
		}

		public Task<T> Single(Expression<Func<T, bool>> predicate)
		{
			return _dbConnection().Single(predicate);
			
		}

		public event EventHandler<T> ObjectInserted;

		public Task Insert(T entity)
		{
			_dbConnection().Insert(entity);

			ObjectInserted?.Invoke(this, entity);

			return Task.CompletedTask;
		}

		public Task Update(T entity)
		{
			_dbConnection().Update(entity);
			return Task.CompletedTask;
		}

		public Task InsertOrReplaceAllWithChildren(IEnumerable<T> entities)
		{
			_dbConnection().InsertOrReplaceAllWithChildren(entities);
			return Task.CompletedTask;
		}

		public Task Delete(T entity)
		{
			_dbConnection().Delete(entity);
			return Task.CompletedTask;
		}

		public Task DeleteWithChildren(T entity)
		{
			_dbConnection().DeleteWithChildren(entity);
			return Task.CompletedTask;
		}
	}
}