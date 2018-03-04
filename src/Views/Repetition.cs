﻿using Flashcards.Services;
using Flashcards.SpacedRepetition.Provider;
using Flashcards.ViewModels;
using Flashcards.ViewModels.Lesson;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using static Flashcards.Views.RepetitionProperties;

namespace Flashcards.Views
{
	public class Repetition : PrismApplication
    {
	    public Repetition(IPlatformInitializer platformInitializer) : base(platformInitializer)
	    {
	    }


	    protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
            containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
        }

	    protected override async void OnInitialized()
        {
            var spacedRepetition = Container.Resolve<ISpacedRepetition>();

	        var sessionNumber = SessionNumber;
	        var flashcards = await spacedRepetition.ChooseFlashcards(sessionNumber);
            var examiner = new Examiner(flashcards);

            await NavigationService.NavigateAsync("NavigationPage/LessonListPage/AskingQuestionsPage",
                new NavigationParameters
                {
                    {
                        "examiner",
                        examiner
                    }
                });
            var results = await examiner.QuestionResults.Task;

            spacedRepetition.RearrangeFlashcards(results, sessionNumber);
        }
    }
}