using System.Threading.Tasks;

namespace Flashcards.SpacedRepetition.Provider
{
    public interface ISpacedRepetitionInitializer
    {
        Task Initialize();
    }
}