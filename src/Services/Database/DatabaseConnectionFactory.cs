using FlashCards.Models;
using SQLite;

namespace FlashCards.Services.Database
{
    public static class DatabaseConnectionFactory
    {
        public static SQLiteAsyncConnection Connect(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

            CreateTablesIfDoNotExist(connection);

            return connection;
        }

        private static void CreateTablesIfDoNotExist(SQLiteAsyncConnection connection)
        {
            connection.CreateTableAsync<FlashCard>();
            connection.CreateTableAsync<Lesson>();
        }
    }
}
