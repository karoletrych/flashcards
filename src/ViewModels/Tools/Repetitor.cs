using System.Linq;
using System.Threading.Tasks;
using Flashcards.Services.Examiner;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;

namespace Flashcards.ViewModels.Tools
{
	public class Repetitor : IRepetitor
	{
		private readonly ISpacedRepetition _spacedRepetition;

		public Repetitor(ISpacedRepetition spacedRepetition)
		{
			_spacedRepetition = spacedRepetition;
		}

		public async Task Repeat(
			INavigationService navigationService,
			string askingQuestionsUri,
			IExaminer examiner)
		{
			await navigationService.NavigateAsync(askingQuestionsUri,
				new NavigationParameters
				{
					{
						"examiner",
						examiner
					}
				});

			examiner.SessionEnded += SubmitResults;
			examiner.Disposed += UnsubscribeResultsSubmit;
		}

		private void UnsubscribeResultsSubmit(object obj, System.EventArgs e)
		{
			((Examiner)(obj)).SessionEnded -= SubmitResults;
		}

		private async void SubmitResults(object obj, QuestionResultsEventArgs args)
		{
			((Examiner)obj).SessionEnded -= SubmitResults;

			var questionResults = args.Results.Select(r =>
				new QuestionResult(r.Question.InternalFlashcard, r.IsKnown));

			await _spacedRepetition.SubmitRepetitionResults(questionResults);
		}
	}
}