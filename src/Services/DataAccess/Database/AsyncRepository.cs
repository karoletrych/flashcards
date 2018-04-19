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

		public async Task<IEnumerable<T>> GetAllWithChildren(Expression<Func<T, bool>> predicate, bool recursive)
		{
			var list = await _dbConnection
				.GetAllWithChildrenAsync(predicate, recursive: recursive)
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

		public async Task<T> Single(Expression<Func<T, bool>> predicate)
		{
			var list = await _dbConnection
				.Table<T>()
				.Where(predicate)
				.Take(1)
				.ToListAsync()
				.ConfigureAwait(false);

			return list.Single();
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

		public event EventHandler<T> ObjectInserted;

		public async Task InsertWithChildren(T entity)
		{
			await _dbConnection.InsertWithChildrenAsync(entity, recursive: true)
				.ConfigureAwait(false);

			ObjectInserted?.Invoke(this, entity);
		}

		public async Task Update(T entity)
		{
			await _dbConnection.InsertOrReplaceWithChildrenAsync(entity, recursive:true).ConfigureAwait(false);
		}

		public async Task UpdateWithChildren(T entity)
		{
			await _dbConnection.UpdateWithChildrenAsync(entity).ConfigureAwait(false);
		}

		public async Task InsertOrReplaceAllWithChildren(IEnumerable<T> entities)
		{
			await _dbConnection.InsertOrReplaceAllWithChildrenAsync(entities)
				.ConfigureAwait(false);
		}

		public async Task Insert(T entity)
		{
			await _dbConnection.InsertAsync(entity).ConfigureAwait(false);
		}

		public async Task Delete(T entity)
		{
			await _dbConnection.DeleteAsync(entity, recursive: true).ConfigureAwait(false);
		}
	}
}