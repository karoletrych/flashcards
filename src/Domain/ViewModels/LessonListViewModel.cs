using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner.Builder;
using Flashcards.SpacedRepetition.Interface;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.Domain.ViewModels
{
	public class LessonListViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly ISpacedRepetition _spacedRepetition;

		public LessonListViewModel()
		{
		}

		public LessonListViewModel(
			IRepository<Lesson> lessonRepository,
			INavigationService navigationService,
			ISpacedRepetition spacedRepetition)
		{
			_lessonRepository = lessonRepository;
			_navigationService = navigationService;
			_spacedRepetition = spacedRepetition;
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			var lessons = (await _lessonRepository.GetAllWithChildren()).ToList();
			var learnedFlashcards = await _spacedRepetition.LearnedFlashcards();
			var viewModels = lessons.Select(l => new LessonViewModel(l, learnedFlashcards));

			Lessons.SynchronizeWith(viewModels);
		}

		public ObservableCollection<LessonViewModel> Lessons { get; } = new ObservableCollection<LessonViewModel>();

		public bool CanNavigate { get; set; } = true;

		public ICommand AddLessonCommand =>
			new DelegateCommand(AddLesson)
				.ObservesCanExecute(() => CanNavigate);

		async void AddLesson()
		{
			CanNavigate = false;
			await _navigationService.NavigateAsync("AddLessonPage");
			CanNavigate = true;
		}

		public ICommand PracticeLessonCommand => new Command<LessonViewModel>(async lesson =>
		{
			var loadedLesson = _lessonRepository.GetWithChildren(l => l.Id == lesson.InternalLesson.Id).Result.Single();
			var flashcards = new FlashcardsInLanguage(
				loadedLesson.FrontLanguage, 
				loadedLesson.BackLanguage,
				loadedLesson.Flashcards);

			var examiner = new ExaminerBuilder()
				.WithFlashcards(new []{ flashcards })
				.WithAskingMode(loadedLesson.AskingMode)
				.WithShuffling(loadedLesson.Shuffle)
				.Build();

			await _navigationService.NavigateAsync("AskingQuestionsPage", new NavigationParameters
			{
				{
					"examiner", examiner
				}
			});
		});

		public ICommand EditLessonCommand =>
			new Command<LessonViewModel>(async lesson =>
			{
				await _navigationService.NavigateAsync("EditLessonPage", new NavigationParameters
				{
					{
						"lessonId", lesson.InternalLesson.Id
					}
				});
			});

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}


#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
	}
}