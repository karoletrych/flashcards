using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Flashcards.Services.Database
{
    public interface IRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindMatching(Expression<Func<T, bool>> predicate);

        Task<int> Insert(T entity);
    }
}