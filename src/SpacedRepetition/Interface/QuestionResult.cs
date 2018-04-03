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
}