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

        public async Task<int> Insert(T entity)
        {
            var id = 0;
            await _dbConnection.RunInTransactionAsync(connection =>
            {
                connection.InsertWithChildren(entity, recursive: true);
                id = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
            });

            ObjectInserted?.Invoke(this, entity);

            return id;
        }
        
        public async Task Update(T entity)
        {
            await _dbConnection.UpdateAsync(entity).ConfigureAwait(continueOnCapturedContext: false);
        }

        public event EventHandler<T> ObjectInserted;


        public async Task Delete(T entity)
        {
            await _dbConnection.DeleteAsync(entity, recursive: true).ConfigureAwait(false);
        }

        public Task UpdateAll(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
