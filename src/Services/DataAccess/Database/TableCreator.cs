using System.Threading.Tasks;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public interface ITableCreator
    {
        Task CreateTable<T>() where T : new();
    }

    public class TableCreator : ITableCreator
    {
        private readonly SQLiteConnection _sqliteConnection;

        public TableCreator(SQLiteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }

        public Task CreateTable<T>() where T : new()
		{
            return Task.FromResult(
                _sqliteConnection.CreateTable<T>());
        }
    }

	public class AsyncTableCreator : ITableCreator
    {
        private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

        public AsyncTableCreator(SQLiteAsyncConnection sqLiteAsyncConnection)
        {
            _sqLiteAsyncConnection = sqLiteAsyncConnection;
        }

        public Task CreateTable<T>() where T : new()
        {
            return _sqLiteAsyncConnection.CreateTableAsync<T>();
        }
    }
}