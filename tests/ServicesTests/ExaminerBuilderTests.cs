using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.Examiner.Builder;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class ExaminerBuilderTests
	{
		private readonly IEnumerable<Lesson> _lessons = new[]
		{
			Lesson.Create(Language.English, Language.Polish, new List<Flashcard>
			{
				Flashcard.Create("Front", "Back")
			})
		};

		[Fact]
		public void WhenBackModeIsSpecified_AllFlashcardsAreInverted()
		{
			var examiner = new ExaminerBuilder()
				.WithAskingMode(AskingMode.Back)
				.WithFlashcards(_lessons.Select(l => new FlashcardsInLanguage(l.FrontLanguage, l.BackLanguage, l.Flashcards)))
				.Build();
			examiner.TryAskNextQuestion(out var question);
			Assert.Equal("Back", question.Front);
			Assert.Equal("Front", question.Back);
		}

	}
}