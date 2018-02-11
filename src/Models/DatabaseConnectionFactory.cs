using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlashCards.Models.Dto;
using SQLite;

namespace FlashCards.Models
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
