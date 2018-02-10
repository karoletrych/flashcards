using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;

namespace FlashCards.Models
{
    public static class DatabaseConnectionFactory
    {
        public static SQLiteAsyncConnection Connect(string databasePath)
        {
//            var databasePath = Path.Combine(
//                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
//                "database.db3");
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
