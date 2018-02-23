using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace Flashcards.Services.Database
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
            var list =
                _dbConnection
                    .Table<T>()
                    .Where(predicate)
                    .ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<int> Insert(T entity)
        {
            _dbConnection.BeginTransaction();
            _dbConnection.Insert(entity);
            var id = _dbConnection.ExecuteScalar<int>("SELECT last_insert_rowid()");
            _dbConnection.Commit();

            return Task.FromResult(id);
        }
    }
}