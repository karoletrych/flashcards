using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.ViewModels;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Prism.Navigation;
using Prism.Services;
using Xunit;

namespace ViewModelsTests
{
    public class LessonListViewModelTests
    {
        private readonly LessonListViewModel _lessonListViewModel;
	    private readonly IRepetitor _repetitor;
	    private readonly INavigationService _navigationService;
	    private readonly IPageDialogService _pageDialogService;
	    private readonly IRepository<Lesson> _lessonRepository;
	    private readonly IRepetitionFlashcardsRetriever _repetitionFlashcardsRetriever;

	    public LessonListViewModelTests()
        {
            _lessonRepository = Substitute.For<IRepository<Lesson>>();
            _navigationService = Substitute.For<INavigationService>();
            var pageDialogService = Substitute.For<IPageDialogService>();
            var flashcardRepository = Substitute.For<IRepository<Flashcard>>();
            var spacedRepetition = Substitute.For<ISpacedRepetition>();
            _repetitor = Substitute.For<IRepetitor>();
            _repetitionFlashcardsRetriever = Substitute.For<IRepetitionFlashcardsRetriever>();


	        
	        _pageDialogService = pageDialogService;
	        _lessonListViewModel = new LessonListViewModel(
	            _lessonRepository, 
	            _navigationService, 
	            _pageDialogService,
                flashcardRepository, 
				new ExaminerBuilder(),
	            spacedRepetition, 
	            _repetitor,
				_repetitionFlashcardsRetriever);
        }

        [Fact]
        public void Dialog_is_displayed_when_repetition_is_tapped_and_there_are_no_flashcards_to_repeat()
        {
	        _lessonRepository.FindAll().Returns(new Lesson[]{});
	        _repetitionFlashcardsRetriever.FlashcardsToAsk().Returns(Task.FromResult(new List<Flashcard>()));

	        _lessonListViewModel.OnNavigatedTo(new NavigationParameters());
			_lessonListViewModel.RunRepetitionCommand.Execute(null);

	        _pageDialogService
		        .Received()
		        .DisplayAlertAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

	    [Fact]
	    public void Repetition_is_executed_with_correct_flashcards_when_repetition_view_is_tapped()
	    {
		    var f1 = new Flashcard { Id = 1 };
		    var f2 = new Flashcard { Id = 2 };
		    var flashcards = new List<Flashcard> { f1, f2 };
		    var lessons = new[]
		    {
			    new Lesson {Id = "1", Flashcards = new List<Flashcard>{f1}},
			    new Lesson {Id = "2", Flashcards = new List<Flashcard>{f2}}
		    };

		    _lessonRepository.FindAll().Returns(lessons);
		    _repetitionFlashcardsRetriever.FlashcardsToAsk().Returns(Task.FromResult(flashcards));

		    _lessonListViewModel.OnNavigatedTo(new NavigationParameters());
			_lessonListViewModel.RunRepetitionCommand.Execute(null);

		    _repetitor
			    .Received()
			    .Repeat(_navigationService, "AskingQuestionsPage", flashcards);
	    }
	}
}