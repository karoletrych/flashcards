using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.Domain.ViewModels
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

		protected bool Equals(LessonViewModel other)
		{
			return string.Equals(Name, other.Name) && string.Equals(LearnedFlashcardsRatioString, other.LearnedFlashcardsRatioString) && LearnedFlashcardsRatio.Equals(other.LearnedFlashcardsRatio);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LessonViewModel) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null
					? Name.GetHashCode()
					: 0);
				hashCode = (hashCode * 397) ^ (LearnedFlashcardsRatioString != null
					           ? LearnedFlashcardsRatioString.GetHashCode()
					           : 0);
				hashCode = (hashCode * 397) ^ LearnedFlashcardsRatio.GetHashCode();
				return hashCode;
			}
		}
	}
}