using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
	public class Repetition : IRepetition
	{
		private readonly ISpacedRepetition _spacedRepetition;
		private readonly ExaminerBuilder _examinerBuilder;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;

		public Repetition(
			ISpacedRepetition spacedRepetition,
			ExaminerBuilder examinerBuilder,
			ISetting<AskingMode> repetitionAskingModeSetting)
		{
			_spacedRepetition = spacedRepetition;
			_examinerBuilder = examinerBuilder;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
		}

		public async Task Repeat(INavigationService navigationService, IEnumerable<Flashcard> flashcardsToAsk)
		{
			var examiner = _examinerBuilder
				.WithFlashcards(flashcardsToAsk)
				.WithRepeatingQuestions(true)
				.WithAskingMode(_repetitionAskingModeSetting.Value)
				.Build();

			await navigationService.NavigateAsync("NavigationPage/LessonListPage/AskingQuestionsPage",
				new NavigationParameters
				{
					{
						"examiner",
						examiner
					}
				});
			var results = await examiner.QuestionResults.Task;

			_spacedRepetition.RearrangeFlashcards(results);
		}
	}
}