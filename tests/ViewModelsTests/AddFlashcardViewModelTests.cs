﻿using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Http;
using Flashcards.ViewModels;
using NSubstitute;
using Prism.Navigation;
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

            _viewModel = new AddFlashcardViewModel(
                _translator,
                repository,
                imageBrowser
            );
        }

        private readonly AddFlashcardViewModel _viewModel;
        private readonly ITranslator _translator;

        
        [Fact]
        private async void WhenFrontTextIsChanged_BackTextTranslationAppears()
        {
            _viewModel.OnNavigatedTo(
                new NavigationParameters
                {
                    {"frontLanguage", Language.English},
                    {"backLanguage", Language.Polish}
                });
            
            _translator
                .Translate(Language.English, Language.Polish, "dog")
                .Returns(new[] {"pies"});

            await TypeInFrontText("dog");

            Assert.Equal("pies", _viewModel.BackText);
        }
        
        [Fact]
        private async void WhenSecondTextIsModifiedByUser_FirstDoesNotChange()
        {
            _viewModel.OnNavigatedTo(
                new NavigationParameters
                {
                    {"frontLanguage", Language.Polish},
                    {"backLanguage", Language.English}
                });
            
            _translator
                .Translate(Language.Polish, Language.English, "nastrojowy")
                .Returns(new[] {"moody"}); // unwanted translation
            
            _translator
                .Translate(Language.English, Language.Polish, "atmospheric")
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