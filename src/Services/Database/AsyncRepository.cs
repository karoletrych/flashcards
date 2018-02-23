using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace Flashcards.Services.Database
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
            var list =  await _dbConnection.Table<T>().ToListAsync();
            return list.AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate)
        {
            var list =  await _dbConnection.Table<T>().Where(predicate).ToListAsync();
            return list.AsEnumerable();
        }

        public async Task<int> Insert(T entity)
        {
            var id = 0;
            await _dbConnection.RunInTransactionAsync(async connection =>
            {
                await _dbConnection.InsertAsync(entity);
                id = await _dbConnection.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
            });
            return id;
        }
    }
}
