using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Flashcards.Services.DataAccess.Database
{
	public class AsyncConnection : IConnection
	{
		private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

		public AsyncConnection(SQLiteAsyncConnection sqLiteAsyncConnection)
		{
			_sqLiteAsyncConnection = sqLiteAsyncConnection;
		}

		public async Task<IEnumerable<T>> GetAllWithChildren<T>(Expression<Func<T, bool>> predicate, bool recursive)
			where T : new()
		{
			return await _sqLiteAsyncConnection.GetAllWithChildrenAsync(predicate, recursive).ConfigureAwait(false);
		}

		public async Task Delete<T>(T entity)
		{
			await _sqLiteAsyncConnection.DeleteAsync(entity).ConfigureAwait(false);
		}

		public async Task DeleteWithChildren<T>(T entity) where T : new()
		{
			await _sqLiteAsyncConnection.DeleteAsync(entity, true).ConfigureAwait(false);
		}

		public async Task Update<T>(T entity)
		{
			await _sqLiteAsyncConnection.UpdateAsync(entity).ConfigureAwait(false);
		}

		public async Task InsertOrReplaceAllWithChildren<T>(IEnumerable<T> entities)
		{
			await _sqLiteAsyncConnection.InsertOrReplaceAllWithChildrenAsync(entities).ConfigureAwait(false);
		}

		public async Task Insert<T>(T entity)
		{
			await _sqLiteAsyncConnection.InsertAsync(entity).ConfigureAwait(false);
		}

		public async Task<T> Single<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			var list = await _sqLiteAsyncConnection
				.Table<T>()
				.Where(predicate)
				.Take(1)
				.ToListAsync()
				.ConfigureAwait(false);
			return list.Single();
		}

		public async Task CreateTable<T>() where T : new()
		{
			await _sqLiteAsyncConnection.CreateTableAsync<T>().ConfigureAwait(false);
		}
	}
}