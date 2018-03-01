using Flashcards.Services;
using Flashcards.SpacedRepetition.Provider;
using Flashcards.ViewModels;
using Flashcards.ViewModels.Lesson;
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
            containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
            containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
        }

        protected override async void OnInitialized()
        {
            var spacedRepetition = Container.Resolve<ISpacedRepetition>();

            var flashcards = await spacedRepetition.ChooseFlashcards();
            var examiner = new Examiner(flashcards);

            await NavigationService.NavigateAsync("NavigationPage/AskingQuestionsPage",
                new NavigationParameters
                {
                    {
                        "examiner",
                        examiner
                    }
                });
            var results = await examiner.QuestionResults.Task;

            spacedRepetition.RearrangeFlashcards(results);

        }


    }
}