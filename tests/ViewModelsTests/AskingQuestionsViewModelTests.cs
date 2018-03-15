using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.ViewModels;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Xunit;

namespace ViewModelsTests
{
    public class AskingQuestionsViewModelTests
    {
        private readonly AskingQuestionsViewModel _askingQuestionsViewModel;
        private readonly INavigationService _navigationService;
	    private Examiner _examiner;

	    public AskingQuestionsViewModelTests()
        {
            _examiner = new Examiner(new[]
            {
	            new Flashcard {Front = "cat", Back = "kot"},
	            new Flashcard {Front = "dog", Back = "pies"},
	            new Flashcard {Front = "duck", Back = "kaczka"}
            });

            _navigationService = Substitute.For<INavigationService>();
            var dialogService = Substitute.For<IPageDialogService>();

            _askingQuestionsViewModel = new AskingQuestionsViewModel(_navigationService, dialogService);
        }

        private async Task NavigateToViewModel()
        {
            await Task.Run(() => _askingQuestionsViewModel.OnNavigatingTo(new NavigationParameters
            {
                {
                    "examiner", _examiner
                }
            }));
        }

        [Fact]
        public async void ShowsFirstQuestionWithHiddenBack_AtStartup()
        {
            await NavigateToViewModel();
            Assert.Equal("cat", _askingQuestionsViewModel.FrontText);
            Assert.Equal("kot", _askingQuestionsViewModel.BackText);
            Assert.False(_askingQuestionsViewModel.FrontIsVisible);
        }

        [Fact]
        public async void AfterShowBackCommand_BackIsVisible_ShowBackButtonIsHidden()
        {
            await NavigateToViewModel();
            _askingQuestionsViewModel.ShowBackCommand.Execute(null);
            Assert.True(_askingQuestionsViewModel.FrontIsVisible);
            Assert.False(_askingQuestionsViewModel.BackIsVisible);
        }

        [Fact]
        public async void AfterUserAnswerCommand_NextQuestionIsShown()
        {
            await NavigateToViewModel();
            _askingQuestionsViewModel.ShowBackCommand.Execute(null);
            _askingQuestionsViewModel.UserAnswerCommand.Execute(true);
            Assert.Equal("dog", _askingQuestionsViewModel.FrontText);
            Assert.Equal("pies", _askingQuestionsViewModel.BackText);
            Assert.False(_askingQuestionsViewModel.FrontIsVisible);
        }

        [Fact]
        public async void WhenAllQuestionsAreAnswered_NavigatesBack()
        {
            await NavigateToViewModel();
            _askingQuestionsViewModel.ShowBackCommand.Execute(null);
            _askingQuestionsViewModel.UserAnswerCommand.Execute(true);
            _askingQuestionsViewModel.ShowBackCommand.Execute(null);
            _askingQuestionsViewModel.UserAnswerCommand.Execute(true);
            _askingQuestionsViewModel.ShowBackCommand.Execute(null);
            _askingQuestionsViewModel.UserAnswerCommand.Execute(true);

            await _navigationService.Received().GoBackAsync();
        }
    }
}
