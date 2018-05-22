using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Domain.ViewModels;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.SpacedRepetition.Interface;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Settings;
using Xunit;

namespace ViewModelsTests
{
	public class RepetitionViewModelTests
	{
		private readonly RepetitionViewModel _sut;
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly IRepetitionExaminerBuilder _repetitionExaminerBuilder;
		private readonly IPageDialogService _pageDialogService;
		private readonly IRepetitor _repetitor;
		private readonly INavigationService _navigationService;
		private readonly ISpacedRepetition _spacedRepetition;

		public RepetitionViewModelTests()
		{
			var setting = Substitute.For<ISetting<int>>();
			_repetitionExaminerBuilder = Substitute.For<IRepetitionExaminerBuilder>();
			_lessonRepository = Substitute.For<IRepository<Lesson>>();
			_pageDialogService = Substitute.For<IPageDialogService>();
			_repetitor = Substitute.For<IRepetitor>();
			_navigationService = Substitute.For<INavigationService>();
			_spacedRepetition = Substitute.For<ISpacedRepetition>();

			_sut = new RepetitionViewModel(setting, _repetitionExaminerBuilder, _lessonRepository, _spacedRepetition, _repetitor, _navigationService, _pageDialogService);
		}

		[Fact]
		public void Dialog_is_displayed_when_repetition_is_tapped_and_there_are_no_flashcards_to_repeat()
		{
			_lessonRepository.GetAllWithChildren().Returns(new Lesson[] { });
			var examiner = Task.FromResult(new Examiner(new List<Question>()) as IExaminer);
			_repetitionExaminerBuilder.BuildExaminer().Returns(examiner);

			_sut.OnNavigatedTo(new NavigationParameters());
			_sut.RunRepetitionCommand.Execute(null);

			_pageDialogService
				.Received()
				.DisplayAlertAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
		}

		[Fact]
		public void Repetition_is_executed_with_correct_flashcards_when_repetition_view_is_tapped()
		{
			var f1 = Flashcard.CreateEmpty();
			var f2 = Flashcard.CreateEmpty();
			var questions = new List<Flashcard> { f1, f2 }.Select(x => new Question(x, Language.English, Language.Polish));
			var lessons = new[]
			{
				Lesson.Create(Language.English, Language.Polish, new List<Flashcard>{f1}),
				Lesson.Create(Language.English, Language.Polish, new List<Flashcard>{f2})
			};

			_lessonRepository.GetAllWithChildren().Returns(lessons);
			var repeatingExaminer = (IExaminer)new Examiner(questions);
			_repetitionExaminerBuilder.BuildExaminer()
				.Returns(Task.FromResult(repeatingExaminer));

			_sut.OnNavigatedTo(new NavigationParameters());
			_sut.RunRepetitionCommand.Execute(null);

			_repetitor
				.Received()
				.Repeat(_navigationService, "AskingQuestionsPage", repeatingExaminer);
		}
	}
}