using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using NSubstitute;
using Xunit;

namespace Flashcards.ServicesTests
{
	public class RepetitionExaminerBuilderTests
	{
		private readonly RepetitionExaminerBuilder _sut;

		private readonly List<Flashcard> _activeFlashcards1 = new List<Flashcard>
		{
			new Flashcard
			{
				Id = 1
			},
			new Flashcard
			{
				Id = 2
			},
		};
		private readonly List<Flashcard> _inactiveFlashcards = new List<Flashcard>
		{
			new Flashcard
			{
				Id = 3
			},
		};
		private readonly List<Flashcard> _activeFlashcards2 = new List<Flashcard>
		{
			new Flashcard
			{
				Id = 4
			},
		};

		private readonly ISpacedRepetition _spacedRepetition;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly IEnumerable<Flashcard> _flashcards;
		private readonly IRepository<Lesson> _lessonRepository;

		public RepetitionExaminerBuilderTests()
		{
			var lessons = new List<Lesson>
			{
				new Lesson
				{
					Id = "1",
					Flashcards = _activeFlashcards1,
					AskInRepetitions = true
				},
				new Lesson
				{
					Id = "2",
					Flashcards = _activeFlashcards2,
					AskInRepetitions = true
				},
				new Lesson
				{
					Id = "3",
					Flashcards = _inactiveFlashcards,
					AskInRepetitions = false
				}
			};
			_flashcards = _activeFlashcards1.AsEnumerable()
				.Concat(_inactiveFlashcards.AsEnumerable())
				.Concat(_activeFlashcards2.AsEnumerable());

			var connection = new Connection(new DatabaseConnectionFactory().CreateInMemoryConnection());
			_lessonRepository = new Repository<Lesson>(() => connection);
			_lessonRepository.InsertOrReplaceAllWithChildren(lessons);
			_spacedRepetition = Substitute.For<ISpacedRepetition>();

			_maximumFlashcardsInRepetitionSetting = Substitute.For<ISetting<int>>();
			_maximumFlashcardsInRepetitionSetting.Value.Returns(20);
			var repetitionAskingModeSetting = Substitute.For<ISetting<AskingMode>>();
			var shuffleRepetitionSetting = Substitute.For<ISetting<bool>>();

			_sut = new RepetitionExaminerBuilder(
				_spacedRepetition, 
				_lessonRepository, 
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
