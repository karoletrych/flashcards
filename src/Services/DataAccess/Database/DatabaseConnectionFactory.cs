using Flashcards.Models;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public class DatabaseConnectionFactory
    {
        private const SQLiteOpenFlags SqliteOpenFlags =
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex;

        public SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SqliteOpenFlags);

            connection.CreateTableAsync<Flashcard>();
            connection.CreateTableAsync<Lesson>();

            return connection;
        }

        public SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SqliteOpenFlags);

            connection.CreateTable<Flashcard>();
            connection.CreateTable<Lesson>();

            return connection;
        }
    }
}