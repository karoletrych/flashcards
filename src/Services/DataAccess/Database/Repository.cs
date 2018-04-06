using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Flashcards.Services.DataAccess.Database
{
	public class Repository<T> : IRepository<T> where T : new()
	{
		private readonly SQLiteConnection _dbConnection;

		public Repository(SQLiteConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public Task<IEnumerable<T>> FindAll()
		{
			var list = _dbConnection
				.GetAllWithChildren<T>(recursive: true)
				.ToList();
			return Task.FromResult(list.AsEnumerable());
		}

		public Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate)
		{
			var list =
				_dbConnection
					.GetAllWithChildren(predicate, recursive: true)
					.ToList();
			return Task.FromResult(list.AsEnumerable());
		}

		public event EventHandler<T> ObjectInserted;

		public Task Insert(T entity)
		{
			_dbConnection.InsertWithChildren(entity, true);

			ObjectInserted?.Invoke(this, entity);

			return Task.CompletedTask;
		}

		public Task Update(T entity)
		{
			_dbConnection.InsertOrReplaceWithChildren(entity, true);
			return Task.CompletedTask;
		}

		public Task InsertOrReplaceAll(IEnumerable<T> entities)
		{
			_dbConnection.InsertOrReplaceAllWithChildren(entities, true);
			return Task.CompletedTask;
		}

		public Task Delete(T entity)
		{
			_dbConnection.Delete(entity, recursive: true);
			return Task.CompletedTask;
		}
	}
}