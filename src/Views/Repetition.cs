using System;
using System.Linq;
using Flashcards.Services;
using Flashcards.SpacedRepetition.Provider;
using Flashcards.ViewModels;
using Flashcards.ViewModels.Lesson;
using Prism.Autofac;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.Views
{
    public class Repetition : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
            containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();

            IocRegistrations.RegisterTypesInIocContainer(containerRegistry.GetBuilder());
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
            
            var repetitionResults = examiner.Questions.Select(q => (q.Flashcard, IsKnown(q)));

            spacedRepetition.RearrangeFlashcards (repetitionResults);
        }

        bool IsKnown(FlashcardQuestion flashcardQuestion)
        {
            switch (flashcardQuestion.Status)
            {
                case QuestionStatus.NotAnswered:
                    throw new ArgumentException("should be answered at this stage");
                case QuestionStatus.Known:
                    return true;
                case QuestionStatus.Unknown:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}