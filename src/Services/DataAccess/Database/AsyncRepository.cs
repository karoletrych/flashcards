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

		public async Task<IEnumerable<T>> GetAllWithChildren(bool recursive)
		{
			var list = await _dbConnection
				.GetAllWithChildrenAsync<T>(recursive: recursive)
				.ConfigureAwait(false);
			return list
				.AsEnumerable();
		}

		public async Task<IEnumerable<T>> Table()
		{
			var list = await _dbConnection
				.Table<T>()
				.ToListAsync()
				.ConfigureAwait(false);

			return list
				.AsEnumerable();
		}

		public async Task<int> Count(Expression<Func<T, bool>> predicate)
		{
			var count = await _dbConnection
				.Table<T>()
				.Where(predicate)
				.CountAsync()
				.ConfigureAwait(false);
			return count;
		}

		public async Task<IEnumerable<T>> FindWhere(Expression<Func<T, bool>> predicate)
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