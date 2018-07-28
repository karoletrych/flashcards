using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flashcards.Services.DataAccess;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Flashcards.Infrastructure.DataAccess
{
	public class AsyncConnection : IConnection
	{
		private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

		public AsyncConnection(SQLiteAsyncConnection sqLiteAsyncConnection)
		{
			_sqLiteAsyncConnection = sqLiteAsyncConnection;
		}

		public async Task<IEnumerable<T>> GetAllWithChildren<T>(Expression<Func<T, bool>> predicate)
			where T : new()
		{
			return await _sqLiteAsyncConnection.GetAllWithChildrenAsync(predicate).ConfigureAwait(false);
		}

		public async Task<IEnumerable<T>> GetUsingSQL<T>(string query, params object[] args) where T : new()
		{
			return await _sqLiteAsyncConnection.QueryAsync<T>(query, args);
		}

		public async Task Delete<T>(T entity)
		{
			await _sqLiteAsyncConnection.DeleteAsync(entity).ConfigureAwait(false);
		}

		public async Task DeleteWithChildren<T>(T entity) where T : new()
		{
			await _sqLiteAsyncConnection.DeleteAsync(entity, true).ConfigureAwait(false);
		}

		public async Task<bool> Any<T>() where T : new()
		{
			var count = await _sqLiteAsyncConnection
				.Table<T>()
				.CountAsync()
				.ConfigureAwait(false);
			return count > 0;
		}

		public async Task<T> SingleWithChildren<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			var list = await _sqLiteAsyncConnection.GetAllWithChildrenAsync(predicate).ConfigureAwait(false);
			return list.Single();
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
			await _sqLiteAsyncConnection
				.CreateTableAsync<T>()
				.ConfigureAwait(false);
		}
	}
}