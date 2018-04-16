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
	        ViewModelMappings.RegisterTypes(containerRegistry);
		}

	    protected override async void OnInitialized()
	    {
			var repetition = Container.Resolve<IRepetitor>();
			var flashcardsToAsk = await Container.Resolve<IRepetitionExaminerBuilder>().Examiner();
			await repetition.Repeat(NavigationService, "NavigationPage/MainPage/AskingQuestionsPage", flashcardsToAsk);
	    }
    }
}