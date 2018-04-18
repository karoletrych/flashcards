using System.Linq;

namespace Flashcards.Services.Examiner
{
	public class CorrectAnswersRatioTracker
	{
		public int Progress { get; private set; }

		private int? _totalNumberOfFlashcards;
		private int _totalAnsweredCorrectly = 0;


		public void UpdateProgress(object sender, QuestionResultsEventArgs e)
		{
			if(_totalNumberOfFlashcards == null)
				_totalNumberOfFlashcards = e.Results.Count;
			_totalAnsweredCorrectly += e.Results.Count(f => f.IsKnown);
			Progress = 100 * _totalAnsweredCorrectly / (_totalNumberOfFlashcards ?? default(int));
		}
	}
}