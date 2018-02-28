using System.Collections.Generic;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public class DatabaseConnectionFactory
    {
        private const SQLiteOpenFlags SqliteOpenFlags =
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.NoMutex;

        private readonly IEnumerable<ITableCreator> _tableCreators;

        public DatabaseConnectionFactory(IEnumerable<ITableCreator> tableCreators)
        {
            _tableCreators = tableCreators;
        }

        public SQLiteAsyncConnection CreateAsyncConnection(string databasePath)
        {
            var connection = new SQLiteAsyncConnection(databasePath,
                SqliteOpenFlags);

            foreach (var tableCreator in _tableCreators)
            {
                tableCreator.CreateTablesAsync(connection);
            }

            return connection;
        }

        public SQLiteConnection CreateConnection(string databasePath)
        {
            var connection = new SQLiteConnection(databasePath,
                SqliteOpenFlags);

            foreach (var tableCreator in _tableCreators)
            {
                tableCreator.CreateTables(connection);
            }

            return connection;
        }
    }
}