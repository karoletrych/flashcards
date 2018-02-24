using Flashcards.Models;
using SQLite;

namespace Flashcards.Services.Database
{
    public static class DatabaseConnectionFactory
    {
        private const SQLiteOpenFlags SqliteOpenFlags =
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex;

        public static SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SqliteOpenFlags);

            connection.CreateTableAsync<Flashcard>();
            connection.CreateTableAsync<Lesson>();

            return connection;
        }

        public static SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SqliteOpenFlags);

            connection.CreateTable<Flashcard>();
            connection.CreateTable<Lesson>();

            return connection;
        }
    }
}