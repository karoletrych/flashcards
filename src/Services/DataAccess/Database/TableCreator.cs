using System.Threading.Tasks;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public interface ITableCreator
    {
        Task<CreateTableResult> CreateTable<T>(CreateFlags createFlags = CreateFlags.None);
    }

    public class TableCreator : ITableCreator
    {
        private readonly SQLiteConnection _sqliteConnection;

        public TableCreator(SQLiteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }

        public Task<CreateTableResult> CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
        {
            return Task.FromResult(
                _sqliteConnection.CreateTable<T>(createFlags));
        }
    }

    internal class AsyncTableCreator
    {
        private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

        public AsyncTableCreator(SQLiteAsyncConnection sqLiteAsyncConnection)
        {
            _sqLiteAsyncConnection = sqLiteAsyncConnection;
        }

        public Task<CreateTableResult> CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
            where T : new()
        {
            return _sqLiteAsyncConnection.CreateTableAsync<T>(createFlags);
        }
    }
}