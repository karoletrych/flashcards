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
		private readonly ExaminerBuilder _examinerBuilder;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
		private readonly ISpacedRepetition _spacedRepetition;

		public Repetitor(
			ISpacedRepetition spacedRepetition,
			ExaminerBuilder examinerBuilder,
			ISetting<AskingMode> repetitionAskingModeSetting)
		{
			_spacedRepetition = spacedRepetition;
			_examinerBuilder = examinerBuilder;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
		}

		public async Task Repeat(
			INavigationService navigationService,
			string askingQuestionsUri,
			ICollection<Flashcard> flashcardsToAsk)
		{
			var examiner = _examinerBuilder
				.WithFlashcards(flashcardsToAsk)
				.WithAskingMode(_repetitionAskingModeSetting.Value)
				.Build();

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
				new QuestionResult(r.Flashcard, r.IsKnown));
			_spacedRepetition.RearrangeFlashcards(questionResults);

			((Examiner) obj).SessionEnded -= ApplyResults; 
		}
	}
}