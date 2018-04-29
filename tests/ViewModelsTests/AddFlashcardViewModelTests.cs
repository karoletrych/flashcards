using System.Threading.Tasks;
using Flashcards.Domain.ViewModels;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Http;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Xunit;

namespace ViewModelsTests
{
    public class AddFlashcardViewModelTests
    {
        public AddFlashcardViewModelTests()
        {
            _translator = Substitute.For<ITranslator>();

            var repository = Substitute.For<IRepository<Flashcard>>();
            var imageBrowser = Substitute.For<IImageBrowser>();
            var lessonRepository = Substitute.For<IRepository<Lesson>>();
            var message = Substitute.For<IMessage>();
	        var pageDialogService = Substitute.For<IPageDialogService>();

            _viewModel = new AddFlashcardViewModel(
                _translator,
                repository,
                imageBrowser,
                lessonRepository,
				message,
				pageDialogService
			);
        }

        private readonly AddFlashcardViewModel _viewModel;
        private readonly ITranslator _translator;

        
        [Fact]
        private async void WhenFrontTextIsChanged_BackTextTranslationAppears()
        {
	        var lesson = new Lesson {FrontLanguage = Language.English, BackLanguage = Language.Polish};
            _viewModel.OnNavigatedTo(
                new NavigationParameters
                {
                    {"lesson", lesson},
                });
            
            _translator
                .TranslateAsync(Language.English, Language.Polish, "dog")
                .Returns(new[] {"pies"});

            await TypeInFrontText("dog");

            Assert.Equal("pies", _viewModel.BackText);
        }
        
        [Fact]
        private async void WhenSecondTextIsModifiedByUser_FirstDoesNotChange()
        {
	        var lesson = new Lesson { FrontLanguage = Language.Polish, BackLanguage = Language.English };

			_viewModel.OnNavigatedTo(
                new NavigationParameters
                {
                    {"lesson", lesson},
                });
            
            _translator
                .TranslateAsync(Language.Polish, Language.English, "nastrojowy")
                .Returns(new[] {"moody"}); // unwanted translation
            
            _translator
                .TranslateAsync(Language.English, Language.Polish, "atmospheric")
                .Returns(new[] {"atmosferyczny"});

            await TypeInFrontText("nastrojowy");            
            await TypeInBackText("atmospheric");  // user corrects translation

            Assert.Equal("nastrojowy", _viewModel.FrontText);
        }

        private async Task TypeInFrontText(string frontText)
        {
            _viewModel.FrontText = frontText;

            await _viewModel.HandleFrontTextChangedByUser();
        }
        
        private async Task TypeInBackText(string backText)
        {
            _viewModel.BackText = backText;

            await _viewModel.HandleBackTextChangedByUser();
        }
    }
}