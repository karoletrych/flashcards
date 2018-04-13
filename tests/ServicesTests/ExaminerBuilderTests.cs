using System.Collections.Generic;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class ExaminerBuilderTests
	{
		private readonly IEnumerable<Lesson> _lessons = new[]
		{
			new Lesson
			{
				Flashcards = new List<Flashcard>
				{
					new Flashcard{Id = 1, Front = "Front", Back = "Back"}
				}
			}
		};

		[Fact]
		public void WhenBackModeIsSpecified_AllFlashcardsAreInverted()
		{
			var examiner = new ExaminerBuilder()
				.WithAskingMode(AskingMode.Back)
				.WithLessons(_lessons)
				.Build();
			examiner.TryAskNextQuestion(out var question);
			Assert.Equal("Back", question.Front);
			Assert.Equal("Front", question.Back);
		}

	}
}