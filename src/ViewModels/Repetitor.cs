using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;

namespace Flashcards.ViewModels
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
			// TODO: memory leak when user navigates back without ending first session
			examiner.SessionEnded += ApplyResults;
		}

		private void ApplyResults(object obj, QuestionResultsEventArgs args)
		{
			var questionResults = args.Results.Select(r =>
				new QuestionResult(r.Question, r.IsKnown));
			_spacedRepetition.RearrangeFlashcards(questionResults);

			((Examiner) obj).SessionEnded -= ApplyResults; 
		}
	}
}