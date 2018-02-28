using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
    public interface ITableCreator
    {
        void CreateTablesAsync(SQLiteAsyncConnection connection);
        void CreateTables(SQLiteConnection connection);
    }
}