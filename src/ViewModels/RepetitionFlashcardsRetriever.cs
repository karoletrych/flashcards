using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.ViewModels
{
	public class RepetitionFlashcardsRetriever
	{
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly ISpacedRepetition _spacedRepetition;

		public RepetitionFlashcardsRetriever(
			ISpacedRepetition spacedRepetition,
			IRepository<Lesson> lessonRepository,
			ISetting<int> maximumFlashcardsInRepetitionSetting)
		{
			_spacedRepetition = spacedRepetition;
			_lessonRepository = lessonRepository;
			_maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;
		}

		public async Task<List<Flashcard>> FlashcardsToAsk()
		{
			var repetitionFlashcards = await _spacedRepetition.CurrentRepetitionFlashcards();
			var activeLessons = await _lessonRepository
				.Where(lesson => lesson.AskInRepetitions);
			var activeFlashcards = activeLessons.SelectMany(lesson => lesson.Flashcards);

			var flashcardsToAsk = repetitionFlashcards
				.Intersect(activeFlashcards)
				.Take(_maximumFlashcardsInRepetitionSetting.Value)
				.ToList();
			return flashcardsToAsk;
		}
	}
}