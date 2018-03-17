using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Interface
{
	public class QuestionResult
	{
		public QuestionResult(Flashcard flashcard, bool isKnown)
		{
			Flashcard = flashcard;
			IsKnown = isKnown;
		}

		public Flashcard Flashcard { get; }
		public bool IsKnown { get; }
	}

    public interface ISpacedRepetition
    {
        Task<IEnumerable<Flashcard>> GetRepetitionFlashcards();
        void RearrangeFlashcards(IEnumerable<QuestionResult> questionResults);
        void Proceed();
	}
}
