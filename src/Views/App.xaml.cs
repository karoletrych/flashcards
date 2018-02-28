using Flashcards.ViewModels;
using Flashcards.ViewModels.Lesson;
using Flashcards.Views.Lesson;
using Prism.Autofac;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Flashcards.Views
{
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
            containerRegistry.RegisterForNavigation<AddLessonPage, AddLessonViewModel>();
            containerRegistry.RegisterForNavigation<AddFlashcardPage, AddFlashcardViewModel>();
            containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();

            IocRegistrations.RegisterTypesInIocContainer(containerRegistry.GetBuilder());
        }

        protected override void OnInitialized()
        {
            InitializeComponent();
            NavigationService.NavigateAsync("NavigationPage/LessonListPage");
        }
    }
}