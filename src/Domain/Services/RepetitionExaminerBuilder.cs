using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.Services.Examiner.Builder;
using Flashcards.SpacedRepetition.Interface;
using Settings;

namespace Flashcards.Services
{
	public interface IRepetitionExaminerBuilder
	{
		Task<IExaminer> BuildExaminer();
	}

	public class RepetitionExaminerBuilder : IRepetitionExaminerBuilder
	{
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
		private readonly ISetting<bool> _shuffleRepetitionsSetting;
		private readonly ISpacedRepetition _spacedRepetition;

		public RepetitionExaminerBuilder(
			ISpacedRepetition spacedRepetition,
			IRepository<Lesson> lessonRepository,
			ISetting<int> maximumFlashcardsInRepetitionSetting,
			ISetting<AskingMode> repetitionAskingModeSetting,
			ISetting<bool> shuffleRepetitionsSetting)
		{
			_spacedRepetition = spacedRepetition;
			_lessonRepository = lessonRepository;
			_maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
			_shuffleRepetitionsSetting = shuffleRepetitionsSetting;
		}

		public async Task<IExaminer> BuildExaminer()
		{
			var flashcardsToRepeat = await _spacedRepetition.CurrentRepetitionFlashcards();
			var activeLessons = await _lessonRepository
				.GetWithChildren(lesson => lesson.AskInRepetitions);
			var flashcardsInLanguages = activeLessons
				.Select(lesson => new FlashcardsInLanguage(
						lesson.FrontLanguage,
						lesson.BackLanguage,
						lesson.Flashcards.Where(f => flashcardsToRepeat.Contains(f)).ToList()
					)
				);

			return new ExaminerBuilder()
				.WithFlashcards(flashcardsInLanguages)
				.WithAskingMode(_repetitionAskingModeSetting.Value)
				.WithShuffling(_shuffleRepetitionsSetting.Value)
				.WithMaximumFlashcards(_maximumFlashcardsInRepetitionSetting.Value)
				.Build();
		}
	}
}