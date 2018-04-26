using System.Linq;

namespace Flashcards.Services.Examiner
{
	public class CorrectAnswersProgressCalculator
	{
		private int? _totalNumberOfFlashcards;
		private int _totalAnsweredCorrectly = 0;

		public int CalculateProgress(QuestionResultsEventArgs e)
		{
			if(_totalNumberOfFlashcards == null)
				_totalNumberOfFlashcards = e.Results.Count;
			_totalAnsweredCorrectly += e.Results.Count(f => f.IsKnown);
			var correctAnswersPercentage = 100 * _totalAnsweredCorrectly / (_totalNumberOfFlashcards ?? default(int));
			return correctAnswersPercentage;
		}
	}
}