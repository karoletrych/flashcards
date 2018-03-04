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
	        _sqliteConnection.CreateTable<T>();
			return Task.CompletedTask;
		}
    }

	public class AsyncTableCreator : ITableCreator
    {
        private readonly SQLiteAsyncConnection _sqLiteAsyncConnection;

        public AsyncTableCreator(SQLiteAsyncConnection sqLiteAsyncConnection)
        {
            _sqLiteAsyncConnection = sqLiteAsyncConnection;
        }

        public async Task CreateTable<T>() where T : new()
        {
            await _sqLiteAsyncConnection.CreateTableAsync<T>().ConfigureAwait(false);
        }
    }
}