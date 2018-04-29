using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Interface
{
	public interface ISpacedRepetition
    {
        Task<IEnumerable<Flashcard>> CurrentRepetitionFlashcards();
	    Task<IEnumerable<Flashcard>> LearnedFlashcards();

        Task SubmitRepetitionResults(IEnumerable<QuestionResult> questionResults);
    }
}
