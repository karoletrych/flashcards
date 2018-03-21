using System;
using Flashcards.Services;
using Flashcards.ViewModels;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public class RepetitionApp : PrismApplication
    {
	    public RepetitionApp(IPlatformInitializer platformInitializer) : base(platformInitializer)
	    {
	    }


	    protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
	        containerRegistry.RegisterForNavigation<NavigationPage>();
	        containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
	        containerRegistry.RegisterForNavigation<AddLessonPage, AddLessonViewModel>();
	        containerRegistry.RegisterForNavigation<AddFlashcardPage, AddFlashcardViewModel>();
	        containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
	        containerRegistry.RegisterForNavigation<EditLessonPage, EditLessonViewModel>();
	        containerRegistry.RegisterForNavigation<SettingsPage>();
		}

	    protected override async void OnInitialized()
	    {
		    try
		    {
			    var repetition = Container.Resolve<Repetition>();
			    var flashcardsToAsk = await Container.Resolve<IRepetitionFlashcardsRetriever>().FlashcardsToAsk();
			    await repetition.Repeat(NavigationService, flashcardsToAsk);
		    }
		    catch (Exception e)
		    {
			    await NavigationService.NavigateAsync("NavigationPage/LessonListPage");
		    }
	    }
    }
}