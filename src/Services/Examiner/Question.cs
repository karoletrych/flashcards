using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
	public class AnsweredQuestion
	{
		public bool IsKnown { get; }
		public Flashcard Flashcard { get; }

		public AnsweredQuestion(Flashcard flashcard, bool isKnown)
		{
			IsKnown = isKnown;
			Flashcard = flashcard;
		}
	}
}