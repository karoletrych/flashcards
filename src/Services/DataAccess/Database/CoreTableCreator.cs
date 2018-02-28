using Flashcards.Models;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public class CoreTableCreator : ITableCreator
    {
        public void CreateTablesAsync(SQLiteAsyncConnection connection)
        {
            connection.CreateTableAsync<Flashcard>();
            connection.CreateTableAsync<Lesson>();
        }

        public void CreateTables(SQLiteConnection connection)
        {
            connection.CreateTable<Flashcard>();
            connection.CreateTable<Lesson>();
        }
    }
}