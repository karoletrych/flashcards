using FlashCards.Models;
using SQLite;

namespace FlashCards.Services.Database
{
    public static class DatabaseConnectionFactory
    {
        public static SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex);

            connection.CreateTableAsync<FlashCard>();
            connection.CreateTableAsync<Lesson>();

            return connection;
        }

        public static SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex);

            connection.CreateTable<FlashCard>();
            connection.CreateTable<Lesson>();

            return connection;
        }
    }
}