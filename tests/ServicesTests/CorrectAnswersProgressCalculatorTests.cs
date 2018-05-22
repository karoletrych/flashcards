using Flashcards.Models;
using Flashcards.Services.Examiner;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class CorrectAnswersProgressCalculatorTests
	{
		private readonly CorrectAnswersProgressCalculator _sut;
		private readonly Examiner _examiner;

		public CorrectAnswersProgressCalculatorTests()
		{
			_examiner = new Examiner(new[]
			{
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish),
				new Question(new Flashcard(), Language.English, Language.Polish)
			});
			_sut = new CorrectAnswersProgressCalculator();
		}

		private void Answer(bool answer, int times)
		{
			for (var i = 0; i < times; i++)
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
				var progress = _sut.CalculateProgress(args);
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
				var progress = _sut.CalculateProgress(args);
				Assert.Equal(40, progress);
			};

			Answer(true, 2);
			Answer(false, 3);
			Answer(false, 3);
		}

		[Fact]
		public void WhenAnsweredAllQuestionsInThirdSession_Gives100PercentRatio()
		{
			var session = 1;
			_examiner.SessionEnded += (sender, args) =>
			{
				var progress = _sut.CalculateProgress(args);
				if(session==3)
					Assert.Equal(100, progress);
				session++;
			};

			Answer(true, 2);
			Answer(false, 3);
			Answer(false, 3);
			Answer(true, 3);
		}
	}
}