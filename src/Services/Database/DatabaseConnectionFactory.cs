using FlashCards.Models;
using SQLite;

namespace FlashCards.Services.Database
{
    public static class DatabaseConnectionFactory
    {
        public static SQLiteConnection Connect(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex);

            CreateTablesIfDoNotExist(connection);

            return connection;
        }

        private static void CreateTablesIfDoNotExist(SQLiteConnection connection)
        {
            connection.CreateTable<FlashCard>();
            connection.CreateTable<Lesson>();
        }
    }
}
