using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.ViewModels;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Xunit;

namespace ViewModelsTests
{
    public class LessonListViewModelTests
    {
        private LessonListViewModel _lessonListViewModel;

        public LessonListViewModelTests()
        {
            var lessonRepository = Substitute.For<IRepository<Lesson>>();
            var navigationService = Substitute.For<INavigationService>();
            var pageDialogService = Substitute.For<IPageDialogService>();
            var flashcardRepository = Substitute.For<IRepository<Flashcard>>();
            var spacedRepetition = Substitute.For<ISpacedRepetition>();
            var repetition = Substitute.For<IRepetition>();


            _lessonListViewModel = new LessonListViewModel(lessonRepository, navigationService, pageDialogService,
                flashcardRepository, new ExaminerBuilder(), spacedRepetition, repetition);
        }

        [Fact]
        public void RepetitionIsRunWhenRepetitionViewIsTapped()
        {
            _lessonListViewModel.RunRepetitionCommand.Execute(null);
        }
    }
}