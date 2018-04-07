using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
	public class Question
	{
		public Language FrontLanguage { get; }
		public Language BackLanguage { get; }
		public Flashcard InternalFlashcard { get; }
		public string Front => InternalFlashcard.Front;
		public string Back => InternalFlashcard.Back;
		public string ImageUrl => InternalFlashcard.ImageUrl;

		public Question(Flashcard flashcard, Language frontLanguage, Language backLanguage)
		{
			InternalFlashcard = flashcard;
			
			FrontLanguage = frontLanguage;
			BackLanguage = backLanguage;
		}
	}

	public class AnsweredQuestion
	{
		public bool IsKnown { get; }
		public Question Question { get; }

		public AnsweredQuestion(Question question, bool isKnown)
		{
			IsKnown = isKnown;
			Question = question;
		}
	}
}