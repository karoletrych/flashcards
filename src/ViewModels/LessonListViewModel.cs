using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.ViewModels.Tools;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
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
			var lessons = (await _lessonRepository.GetAllWithChildren(true)).ToList();

			Lessons.SynchronizeWith(
				lessons,
				l => new LessonViewModel(l, _spacedRepetition.LearnedFlashcards));
		}

		public ObservableCollection<LessonViewModel> Lessons { get; } = new ObservableCollection<LessonViewModel>();

		public ICommand AddLessonCommand =>
			new Command(() =>
			{
				_navigationService.NavigateAsync("AddLessonPage");
			});


		public ICommand PracticeLessonCommand => new Command<LessonViewModel>(async lesson =>
		{
			var loadedLesson = _lessonRepository.FindWhere(l => l.Id == lesson.InternalLesson.Id).Result.Single();

			var examiner = new ExaminerBuilder()
				.WithLessons(new []{ loadedLesson })
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