using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Provider
{
    public interface ISpacedRepetition
    {
        Task<IEnumerable<Flashcard>> ChooseFlashcards();
        void RearrangeFlashcards(IEnumerable<(Flashcard, bool)> questionResults);
    }
}
