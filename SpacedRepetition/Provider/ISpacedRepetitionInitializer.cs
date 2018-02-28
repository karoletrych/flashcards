using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Provider
{
    public interface ISpacedRepetitionInitializer
    {
        void Initialize(Flashcard flashcard);
    }
}