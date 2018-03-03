using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
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
            var list =  await _dbConnection
                .GetAllWithChildrenAsync<T>(recursive: true)
                .ConfigureAwait(continueOnCapturedContext: false);
            return list
                .AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate)
        {
            var list =  await _dbConnection
                .GetAllWithChildrenAsync(predicate, recursive: true)
                .ConfigureAwait(continueOnCapturedContext: false);
            return list.AsEnumerable();
        }

        public async Task Insert(T entity)
        {
            await _dbConnection.InsertWithChildrenAsync(entity, recursive: true);

            ObjectInserted?.Invoke(this, entity);
        }
        
        public async Task Update(T entity)
        {
            await _dbConnection.InsertOrReplaceAsync(entity).ConfigureAwait(continueOnCapturedContext: false);
        }

        public event EventHandler<T> ObjectInserted;


	    public async Task UpdateAll(IEnumerable<T> entities)
	    {
		    await _dbConnection.InsertAllWithChildrenAsync(entities)
			    .ConfigureAwait(continueOnCapturedContext: false);
	    }

	    public async Task Delete(T entity)
        {
            await _dbConnection.DeleteAsync(entity, recursive: true).ConfigureAwait(false);
        }
    }
}
