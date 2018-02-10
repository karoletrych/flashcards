using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace FlashCards.Models
{
    public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate);

        void Insert(T entity);
    }

    public class Repository<T> : IRepository<T> where T : new()
    {
        private readonly SQLiteAsyncConnection _dbConnection;

        public Repository(SQLiteAsyncConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<T>> FindAll()
        {
            var list = await _dbConnection.Table<T>().ToListAsync();
            return list.AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate)
        {
            var list = await _dbConnection.Table<T>().Where(predicate).ToListAsync();
            return list.AsEnumerable();
        }

        public async void Insert(T entity)
        {
            await _dbConnection.InsertAsync(entity);
        }
    }
}
