using System.Threading.Tasks;
using Flashcards.Models;
using SQLite;

namespace Flashcards.Infrastructure.DataAccess
{
    public class DatabaseConnectionFactory
    {
	    private const SQLiteOpenFlags SqliteOpenFlags =
		    SQLiteOpenFlags.Create | 
		    SQLiteOpenFlags.ReadWrite | 
		    SQLiteOpenFlags.FullMutex;

        public SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(
	            databasePath,
                SqliteOpenFlags);

	        Task.Run(() => connection.CreateTableAsync<Flashcard>()).Wait();
	        Task.Run(() => connection.CreateTableAsync<Lesson>()).Wait();

            return connection;
        }

        public SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(
	            databasePath,
                SqliteOpenFlags);

			connection.CreateTable<Flashcard>();
            connection.CreateTable<Lesson>();

            return connection;
        }

	    public SQLiteConnection CreateInMemoryConnection()
	    {
		    return CreateConnection(":memory:");
	    }
    }
}