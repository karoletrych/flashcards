using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.Domain.ViewModels
{
	public class FlashcardListViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly INavigationService _navigationService;
		private Lesson _lesson;

		public FlashcardListViewModel(
			IRepository<Flashcard> flashcardRepository,
			INavigationService navigationService)
		{
			_flashcardRepository = flashcardRepository;
			_navigationService = navigationService;
		}

		public FlashcardListViewModel()
		{
		}

		public ObservableCollection<Flashcard> Flashcards { get; } = new ObservableCollection<Flashcard>();

		public ICommand DeleteFlashcardCommand => new Command<string>(async flashcardId =>
		{
			var flashcardToRemove = Flashcard.CreateEmpty();
			await _flashcardRepository.Delete(flashcardToRemove);
			Flashcards.Remove(flashcardToRemove);
		});

		public ICommand AddFlashcardsCommand => new Command(() =>
			_navigationService.NavigateAsync("AddFlashcardPage",
				new NavigationParameters
				{
					{
						"lesson", _lesson
					}
				}));

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			_lesson = (Lesson) parameters["lesson"];
			var flashcards = await _flashcardRepository.GetWithChildren(f => f.LessonId == _lesson.Id);

			Flashcards.SynchronizeWith(flashcards);
			SortByCreationDate.Execute(null);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#region Sorting

		private bool _sortingByFrontTextAscending;
		private bool _sortingByBackTextAscending;
		private bool _sortingByCreationDateAscending;

		public ICommand SortByFront => new Command(() =>
		{
			_sortingByFrontTextAscending = !_sortingByFrontTextAscending;
			Flashcards.Sort((f1, f2) => string.Compare(f1.Front, f2.Front, StringComparison.CurrentCultureIgnoreCase), _sortingByFrontTextAscending);
		});

		public ICommand SortByBack => new Command(() =>
		{
			_sortingByBackTextAscending = !_sortingByBackTextAscending;
			Flashcards.Sort((f1, f2) =>
				string.Compare(f1.Back, f2.Back, StringComparison.CurrentCultureIgnoreCase), _sortingByBackTextAscending);
		});

		public ICommand SortByCreationDate => new Command(() =>
		{
			_sortingByCreationDateAscending = !_sortingByCreationDateAscending;
			Flashcards.Sort((f1, f2) => DateTime.Compare(f1.Created, f2.Created), _sortingByCreationDateAscending);
		});

		#endregion
	}
}