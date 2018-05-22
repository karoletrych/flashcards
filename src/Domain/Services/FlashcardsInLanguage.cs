using System.Collections.Generic;
using Flashcards.Models;

namespace Flashcards.Services
{
	public class FlashcardsInLanguage
	{
		public FlashcardsInLanguage(Language frontLanguage, Language backLanguage, IEnumerable<Flashcard> flashcards)
		{
			FrontLanguage = frontLanguage;
			BackLanguage = backLanguage;
			Flashcards = flashcards;
		}

		public Language FrontLanguage { get; }
		public Language BackLanguage { get; }
		public IEnumerable<Flashcard> Flashcards { get; }
	}
}