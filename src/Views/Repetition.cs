using System.Linq;
using Flashcards.Services;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.ViewModels;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

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
	        containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
	        containerRegistry.RegisterForNavigation<AddLessonPage, AddLessonViewModel>();
	        containerRegistry.RegisterForNavigation<AddFlashcardPage, AddFlashcardViewModel>();
	        containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
	        containerRegistry.RegisterForNavigation<EditLessonPage, EditLessonViewModel>();
	        containerRegistry.RegisterForNavigation<SettingsPage>();
		}

	    protected override async void OnInitialized()
        {
            var spacedRepetition = Container.Resolve<ISpacedRepetition>();

	        var sessionNumber = Settings.RepetitionSessionNumber;
	        var flashcards = await spacedRepetition.ChooseFlashcards(sessionNumber);
	        var flashcardList = flashcards.ToList();

	        if (flashcardList.Any())
	        {
		        var examiner = new Examiner(flashcardList);

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
	        else
	        {
		        await NavigationService.NavigateAsync("NavigationPage/LessonListPage");
	        }
        }
    }
}