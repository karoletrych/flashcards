using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace FlashCards.Services.Database
{
    public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate);

        void Insert(T entity);
    }

    public class Repository<T> : IRepository<T> where T : new()
    {
        private readonly SQLiteConnection _dbConnection;

        public Repository(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<T>> FindAll()
        {
            var list =  _dbConnection.Table<T>().ToList();
            return list.AsEnumerable();
        }

        public async Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate)
        {
            var list =  _dbConnection.Table<T>().Where(predicate).ToList();
            return list.AsEnumerable();
        }

        public async void Insert(T entity)
        {
             _dbConnection.Insert(entity);
        }
    }
}
