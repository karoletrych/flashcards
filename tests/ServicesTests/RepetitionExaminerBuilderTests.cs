using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.SpacedRepetition.Interface;
using NSubstitute;
using Settings;
using SQLite;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class RepetitionExaminerBuilderTests
	{
		private readonly RepetitionExaminerBuilder _sut;

		private readonly List<Flashcard> _activeFlashcards1 = new List<Flashcard>
		{
			Flashcard.CreateEmpty(),
			Flashcard.CreateEmpty()
		};
		private readonly List<Flashcard> _inactiveFlashcards = new List<Flashcard>
		{
			Flashcard.CreateEmpty(),
		};
		private readonly List<Flashcard> _activeFlashcards2 = new List<Flashcard>
		{
			Flashcard.CreateEmpty(),
		};

		private readonly ISpacedRepetition _spacedRepetition;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly IEnumerable<Flashcard> _flashcards;

		public RepetitionExaminerBuilderTests()
		{
			var lessons = new List<Lesson>
			{
				new Lesson
				{
					Id = "1",
					FrontLanguage = Language.English,
					BackLanguage = Language.Polish,
					Flashcards = _activeFlashcards1,
					AskInRepetitions = true
				},
				new Lesson
				{
					Id = "2",

					FrontLanguage = Language.English,
					BackLanguage = Language.Polish,
					Flashcards = _activeFlashcards2,
					AskInRepetitions = true
				},
				new Lesson
				{
					Id = "3",
					FrontLanguage = Language.English,
					BackLanguage = Language.Polish,
					Flashcards = _inactiveFlashcards,
					AskInRepetitions = false
				}
			};
			_flashcards = _activeFlashcards1.AsEnumerable()
				.Concat(_inactiveFlashcards.AsEnumerable())
				.Concat(_activeFlashcards2.AsEnumerable());

			var connection = new Connection(
				new DatabaseConnectionFactory().CreateInMemoryConnection());
			var lessonRepository = new Repository<Lesson>(() => connection);
			lessonRepository.InsertOrReplaceAllWithChildren(lessons).Wait();

			_spacedRepetition = Substitute.For<ISpacedRepetition>();

			_maximumFlashcardsInRepetitionSetting = Substitute.For<ISetting<int>>();
			_maximumFlashcardsInRepetitionSetting.Value.Returns(20);
			var repetitionAskingModeSetting = Substitute.For<ISetting<AskingMode>>();
			var shuffleRepetitionSetting = Substitute.For<ISetting<bool>>();

			_sut = new RepetitionExaminerBuilder(
				_spacedRepetition, 
				lessonRepository, 
				_maximumFlashcardsInRepetitionSetting,
				repetitionAskingModeSetting,
				shuffleRepetitionSetting);
		}


		[Fact]
		public async void BuildExaminer_ReturnsOnlyFlashcardsFromActiveLessons()
		{
			 _spacedRepetition.CurrentRepetitionFlashcards().Returns(
				Task.FromResult(_flashcards));

			var examiner = await _sut.BuildExaminer();

			Assert.Equal(
				_activeFlashcards1.Count + _activeFlashcards2.Count,
				examiner.QuestionsCount);
		}

		[Fact]
		public async void BuildExaminer_ReturnsNumberOfFlashcardsLimitedToMaximumSetting()
		{
			_maximumFlashcardsInRepetitionSetting.Value.Returns(1);

			_spacedRepetition.CurrentRepetitionFlashcards().Returns(
				Task.FromResult(_activeFlashcards1.AsEnumerable()));

			var examiner = await _sut.BuildExaminer();

			Assert.Equal(1, examiner.QuestionsCount);
		}

		[Fact]
		public async void BuildExaminer_ReturnsOnlyFlashcardsFromCurrentRepetition()
		{
			_spacedRepetition.CurrentRepetitionFlashcards().Returns(
				Task.FromResult(_activeFlashcards2.AsEnumerable()));

			var examiner = await _sut.BuildExaminer();

			Assert.Equal(1, examiner.QuestionsCount);
		}
	}
}
