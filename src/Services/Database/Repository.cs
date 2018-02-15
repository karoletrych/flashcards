using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace FlashCards.Services.Database
{
    public class Repository<T> : IRepository<T> where T : new()
    {
        private readonly SQLiteConnection _dbConnection;

        public Repository(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<IEnumerable<T>> FindAll()
        {
            var list = _dbConnection.Table<T>().ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate)
        {
            var list = _dbConnection.Table<T>().Where(predicate).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public void Insert(T entity)
        {
            _dbConnection.Insert(entity);
        }
    }
}