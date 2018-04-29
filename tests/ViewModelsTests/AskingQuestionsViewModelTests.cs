using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Flashcards.Services.Examiner.Builder;
using Flashcards.ViewModels;
using FluentAssertions;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using Xunit;

namespace ViewModelsTests
{
	public class AskingQuestionsViewModelTests
	{
		public AskingQuestionsViewModelTests()
		{
			_lesson = new Lesson
			{
				Flashcards = new List<Flashcard>
				{
					new Flashcard {Front = "cat", Back = "kot"},
					new Flashcard {Front = "dog", Back = "pies"},
					new Flashcard {Front = "duck", Back = "kaczka"}
				}
			};

			_examiner = new ExaminerBuilder().WithLessons(new[] {_lesson}).Build();

			_navigationService = Substitute.For<INavigationService>();
			_pageDialogService = Substitute.For<IPageDialogService>();
			var textToSpeech = Substitute.For<ITextToSpeech>();
			_askingQuestionsViewModel =
				new AskingQuestionsViewModel(_navigationService, _pageDialogService, textToSpeech, examiner => new CorrectAnswersProgressCalculator());
		}

		private readonly AskingQuestionsViewModel _askingQuestionsViewModel;
		private readonly INavigationService _navigationService;
		private readonly IExaminer _examiner;

		private readonly Lesson _lesson;
		private readonly IPageDialogService _pageDialogService;

		private async Task NavigateToViewModel()
		{
			await Task.Run(() => _askingQuestionsViewModel.OnNavigatedTo(new NavigationParameters
			{
				{
					"examiner", _examiner
				}
			}));
		}

		[Fact]
		public async void After_answering_first_question_first_element_of_QuestionStatuses_is_changed()
		{
			await NavigateToViewModel();

			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			_askingQuestionsViewModel.UserAnswerCommand.Execute(true);
			Assert.Equal(_askingQuestionsViewModel.QuestionStatuses.Count, _lesson.Flashcards.Count);
			Assert.Equal(Color.LawnGreen, _askingQuestionsViewModel.QuestionStatuses.First().Color);
		}

		[Fact]
		public async void AfterShowBackCommand_BackIsVisible()
		{
			await NavigateToViewModel();
			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			Assert.True(_askingQuestionsViewModel.BackIsVisible);
		}

		[Fact]
		public async void AfterUserAnswerCommand_NextQuestionIsShown()
		{
			await NavigateToViewModel();
			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			_askingQuestionsViewModel.UserAnswerCommand.Execute(true);
			Assert.Equal("dog", _askingQuestionsViewModel.FrontText);
			Assert.Equal("pies", _askingQuestionsViewModel.BackText);
			Assert.False(_askingQuestionsViewModel.BackIsVisible);
		}

		[Fact]
		public async void at_startup_first_question_with_hidden_back_is_displayed()
		{
			await NavigateToViewModel();
			Assert.Equal("cat", _askingQuestionsViewModel.FrontText);
			Assert.Equal("kot", _askingQuestionsViewModel.BackText);
			Assert.False(_askingQuestionsViewModel.BackIsVisible);
		}

		[Fact]
		public async void
			at_startup_QuestionStatuses_has_number_of_gray_items_equal_to_number_of_flashcards()
		{
			await NavigateToViewModel();
			Assert.Equal(_askingQuestionsViewModel.QuestionStatuses.Count, _lesson.Flashcards.Count);
			Assert.All(_askingQuestionsViewModel.QuestionStatuses,
				item => Assert.Equal(Color.Gray, item.Color));
		}

		[Fact]
		public async void when_all_questions_are_answered_navigates_back()
		{
			await NavigateToViewModel();
			AnswerQuestions();

			await _navigationService.Received().GoBackAsync();
		}

		[Fact]
		public async void after_first_session_shows_correct_percentage_of_answered()
		{
			await NavigateToViewModel();
			AnswerQuestions();

			await _pageDialogService.Received().DisplayAlertAsync(
				Arg.Any<string>(),
				Arg.Is<string>(x => x.EndsWith("100%")),
				Arg.Any<string>());
		}

		private void AnswerQuestions()
		{
			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			_askingQuestionsViewModel.UserAnswerCommand.Execute(true);
			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			_askingQuestionsViewModel.UserAnswerCommand.Execute(true);
			_askingQuestionsViewModel.ShowBackCommand.Execute(null);
			_askingQuestionsViewModel.UserAnswerCommand.Execute(true);
		}
	}
}