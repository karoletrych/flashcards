using Flashcards.Models;
using SQLite;

namespace Flashcards.Services.Database
{
    public static class DatabaseConnectionFactory
    {
        public static SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex);

            connection.CreateTableAsync<Flashcard>();
            connection.CreateTableAsync<Lesson>();

            return connection;
        }

        public static SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex);

            connection.CreateTable<Flashcard>();
            connection.CreateTable<Lesson>();

            return connection;
        }
    }
}