using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.Examiner;
using Flashcards.Settings;
using Flashcards.ViewModels;
using NSubstitute;
using Prism.Navigation;
using Xunit;

namespace ViewModelsTests
{
	public class RepetitionViewModelTests
	{
		private RepetitionViewModel _sut;

		public RepetitionViewModelTests()
		{
			var setting = Substitute.For<ISetting<int>>();
			var repetitionExaminerBuilder = Substitute.For<IRepetitionExaminerBuilder>();

			_sut = new RepetitionViewModel(setting);
		}

		[Fact]
		public void Dialog_is_displayed_when_repetition_is_tapped_and_there_are_no_flashcards_to_repeat()
		{
			_lessonRepository.FindAll().Returns(new Lesson[] { });
			var examiner = Task.FromResult(new Examiner(new List<Question>()) as IExaminer);
			_repetitionExaminerBuilder.Examiner().Returns(examiner);

			_lessonListViewModel.OnNavigatedTo(new NavigationParameters());
			_lessonListViewModel.RunRepetitionCommand.Execute(null);

			_pageDialogService
				.Received()
				.DisplayAlertAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
		}

		[Fact]
		public void Repetition_is_executed_with_correct_flashcards_when_repetition_view_is_tapped()
		{
			var f1 = new Flashcard { Id = 1 };
			var f2 = new Flashcard { Id = 2 };
			var questions = new List<Flashcard> { f1, f2 }.Select(x => new Question(x, Language.English, Language.Polish));
			var lessons = new[]
			{
				new Lesson {Id = "1", Flashcards = new List<Flashcard>{f1}},
				new Lesson {Id = "2", Flashcards = new List<Flashcard>{f2}}
			};

			_lessonRepository.FindAll().Returns(lessons);
			var repeatingExaminer = (IExaminer)new Examiner(questions);
			_repetitionExaminerBuilder.Examiner()
				.Returns(Task.FromResult(repeatingExaminer));

			_lessonListViewModel.OnNavigatedTo(new NavigationParameters());
			_lessonListViewModel.RunRepetitionCommand.Execute(null);

			_repetitor
				.Received()
				.Repeat(_navigationService, "AskingQuestionsPage", repeatingExaminer);
		}
	}
}