using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess.Database
{
    public interface ITableCreator
    {
        Task CreateTable<T>() where T : new();
    }
}