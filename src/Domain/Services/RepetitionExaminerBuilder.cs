using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Infrastructure.Settings;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.Services.Examiner.Builder;
using Flashcards.SpacedRepetition.Interface;

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
		private readonly ISpacedRepetition _spacedRepetition;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
		private readonly ISetting<bool> _shuffleRepetitionsSetting;

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
			var lessons = activeLessons
				.Select(lesson => WithFlashcards(lesson,
					lesson.Flashcards.Where(f => flashcardsToRepeat.Contains(f)).ToList()));

			return new ExaminerBuilder()
				.WithLessons(lessons)
				.WithAskingMode(_repetitionAskingModeSetting.Value)
				.WithShuffling(_shuffleRepetitionsSetting.Value)
				.WithMaximumFlashcards(_maximumFlashcardsInRepetitionSetting.Value)
				.Build();
		}

		private static Lesson WithFlashcards(Lesson lesson, List<Flashcard> flashcards)
		{
			return new Lesson
			{
				Id = lesson.Id,
				AskingMode = lesson.AskingMode,
				AskInRepetitions = lesson.AskInRepetitions,
				FrontLanguage = lesson.FrontLanguage,
				BackLanguage = lesson.BackLanguage,
				Name = lesson.Name,
				Shuffle = lesson.Shuffle,
				Flashcards = flashcards
			};
		}
	}
}

