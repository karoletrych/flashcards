using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Flashcards.Infrastructure.DataAccess
{
	public class Connection : IConnection
	{
		private readonly SQLiteConnection _dbConnection;

		public Connection(SQLiteConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public Task<IEnumerable<T>> GetAllWithChildren<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			var list = _dbConnection.GetAllWithChildren(predicate);
			return Task.FromResult(list.AsEnumerable());
		}

		public Task<T> Single<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			var single = _dbConnection
				.Table<T>()
				.Where(predicate)
				.Take(1)
				.ToList()
				.Single();
			return Task.FromResult(single);
		}

		Task IDatabase.Insert<T>(T entity)
		{
			_dbConnection.Insert(entity);
			return Task.CompletedTask;
		}

		Task IDatabase.Update<T>(T entity)
		{
			_dbConnection.InsertOrReplaceWithChildren(entity, true);
			return Task.CompletedTask;
		}

		Task IDatabase.InsertOrReplaceAllWithChildren<T>(IEnumerable<T> entities)
		{
			_dbConnection.InsertOrReplaceAllWithChildren(entities, true);
			return Task.CompletedTask;
		}

		Task IDatabase.Delete<T>(T entity)
		{
			_dbConnection.Delete(entity, recursive: true);
			return Task.CompletedTask;
		}

		public Task DeleteWithChildren<T>(T entity) where T : new()
		{
			_dbConnection.Delete(entity, true);
			return Task.CompletedTask;
		}

		public Task CreateTable<T>() where T : new()
		{
			_dbConnection.CreateTable<T>();
			return Task.CompletedTask;
		}
	}
}