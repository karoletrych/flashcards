using System.Threading.Tasks;

namespace Flashcards.Services.DataAccess
{
    public interface ITableCreator
    {
        Task CreateTable<T>() where T : new();
    }
}