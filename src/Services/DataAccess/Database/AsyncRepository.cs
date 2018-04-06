using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Flashcards.Services.DataAccess.Database
{
	public class AsyncRepository<T> : IRepository<T> where T : new()
	{
		private readonly SQLiteAsyncConnection _dbConnection;

		public AsyncRepository(SQLiteAsyncConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<IEnumerable<T>> FindAll()
		{
			var list = await _dbConnection
				.GetAllWithChildrenAsync<T>(recursive: true)
				.ConfigureAwait(false);
			return list
				.AsEnumerable();
		}

		public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate)
		{
			var list = await _dbConnection
				.GetAllWithChildrenAsync(predicate, recursive: true)
				.ConfigureAwait(false);
			return list.AsEnumerable();
		}

		public event EventHandler<T> ObjectInserted;

		public async Task Insert(T entity)
		{
			await _dbConnection.InsertWithChildrenAsync(entity, recursive: true)
				.ConfigureAwait(false);

			ObjectInserted?.Invoke(this, entity);
		}

		public async Task Update(T entity)
		{
			await _dbConnection.InsertOrReplaceWithChildrenAsync(entity, recursive:true).ConfigureAwait(false);
		}


		public async Task InsertOrReplaceAll(IEnumerable<T> entities)
		{
			await _dbConnection.InsertOrReplaceAllWithChildrenAsync(entities)
				.ConfigureAwait(false);
		}

		public async Task Delete(T entity)
		{
			await _dbConnection.DeleteAsync(entity, recursive: true).ConfigureAwait(false);
		}
	}
}