using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.ViewModels
{
	public class LessonViewModel
	{
		public LessonViewModel(Lesson internalLesson, IEnumerable<Flashcard> learnedFlashcards)
		{
			var lessonFlashcardsCount = internalLesson.Flashcards.Count;
			var learnedFlashcardsCount = internalLesson.Flashcards.Intersect(learnedFlashcards).Count();

			FrontLanguage = internalLesson.FrontLanguage;
			BackLanguage = internalLesson.BackLanguage;
			Name = internalLesson.Name;
			LearnedFlashcardsRatioString = learnedFlashcardsCount + "/" + lessonFlashcardsCount;
			LearnedFlashcardsRatio = (double)learnedFlashcardsCount / lessonFlashcardsCount;
			InternalLesson = internalLesson;
		}

		public string Name { get; }
		public Language BackLanguage { get; }
		public Language FrontLanguage { get; }
		public string LearnedFlashcardsRatioString { get; }
		public double LearnedFlashcardsRatio { get; }
		public Lesson InternalLesson { get; }
	}
}