using Flashcards.Domain.ViewModels;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;
using NSubstitute;
using Prism.Navigation;
using Prism.Services;
using Settings;

namespace ViewModelsTests
{
    public class LessonListViewModelTests
    {
        private readonly LessonListViewModel _lessonListViewModel;
	    private readonly IRepetitor _repetitor;
	    private readonly INavigationService _navigationService;
	    private readonly IPageDialogService _pageDialogService;
	    private readonly IRepository<Lesson> _lessonRepository;
	    private readonly IRepetitionExaminerBuilder _repetitionExaminerBuilder;

	    public LessonListViewModelTests()
        {
            _lessonRepository = Substitute.For<IRepository<Lesson>>();
            _navigationService = Substitute.For<INavigationService>();
            var pageDialogService = Substitute.For<IPageDialogService>();
            var flashcardRepository = Substitute.For<IRepository<Flashcard>>();
            var spacedRepetition = Substitute.For<ISpacedRepetition>();
            _repetitor = Substitute.For<IRepetitor>();
            _repetitionExaminerBuilder = Substitute.For<IRepetitionExaminerBuilder>();
	        var streakDaysSetting = Substitute.For<ISetting<int>>();


			_pageDialogService = pageDialogService;
	        _lessonListViewModel = new LessonListViewModel(
	            _lessonRepository, 
	            _navigationService, 
	            spacedRepetition);
        }

		//TODO: add tests

	}
}