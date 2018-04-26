using Flashcards.Models;
using Flashcards.Services.Examiner;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class CorrectAnswersRatioTrackerTests
	{
		private readonly CorrectAnswersProgressCalculator _calculator;
		private readonly Examiner _examiner;

		public CorrectAnswersRatioTrackerTests()
		{
			_examiner = new Examiner(new[]
			{
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish)
			});
		}

		private void Answer(bool answer, int times)
		{
			for (int i = 0; i < times; i++)
			{
				_examiner.TryAskNextQuestion(out var _);
				_examiner.Answer(answer);
			}
		}

		[Fact]
		public void WhenAnswered2QuestionsOutOf5_Gives40PercentRatio()
		{
			_examiner.SessionEnded += (sender, args) =>
			{
				var progress = _calculator.CalculateProgress(args);
				Assert.Equal(40, progress);
			};

			Answer(true, 2);
			Answer(false, 3);
		}

		[Fact]
		public void WhenAnswered2QuestionsOutOf5_AndAllAnswersAreIncorrectInNextSession_Gives40PercentRatio()
		{
			_examiner.SessionEnded += (sender, args) =>
			{
				var progress = _calculator.CalculateProgress(args);
				Assert.Equal(40, progress);
			};

			Answer(true, 2);
			Answer(false, 3);
			Answer(false, 3);
		}

		[Fact]
		public void WhenAnsweredAllQuestionsInThirdSession_Gives100PercentRatio()
		{
			_examiner.SessionEnded += (sender, args) =>
			{
				var progress = _calculator.CalculateProgress(args);
				Assert.Equal(100, progress);
			};

			Answer(true, 2);
			Answer(false, 3);
			Answer(false, 3);
			Answer(true, 3);
		}
	}
}