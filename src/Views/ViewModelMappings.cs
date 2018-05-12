using Flashcards.Domain.ViewModels;
using Prism.Ioc;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public static class ViewModelMappings
	{
		public static void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<MainPage, MainViewModel>();
			containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
			containerRegistry.RegisterForNavigation<EditLessonPage, EditLessonViewModel>();
			containerRegistry.RegisterForNavigation<AddLessonPage, AddLessonViewModel>();
			containerRegistry.RegisterForNavigation<FlashcardListPage, FlashcardListViewModel>();
			containerRegistry.RegisterForNavigation<AddFlashcardPage, AddFlashcardViewModel>();

			containerRegistry.RegisterForNavigation<SettingsPage>();
		}
	}
}