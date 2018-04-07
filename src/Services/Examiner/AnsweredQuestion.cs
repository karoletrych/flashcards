using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
	public class Question : Flashcard
	{
		public Language FrontLanguage { get; }
		public Language BackLanguage { get; }

		public Question(Flashcard flashcard, Language frontLanguage, Language backLanguage)
		{
			Id = flashcard.Id;
			Front = flashcard.Front;
			Back = flashcard.Back;
			ImageUrl = flashcard.ImageUrl;
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