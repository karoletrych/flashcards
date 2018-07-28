using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.Domain.ViewModels
{
	public class FlashcardViewModel
	{

		public FlashcardViewModel(Flashcard flashcard, KnowledgeLevel knowledgeLevel)
		{
			Flashcard = flashcard;
			KnowledgeLevelColor = 
				knowledgeLevel == KnowledgeLevel.Known ? Color.GreenYellow :
				knowledgeLevel == KnowledgeLevel.Medium ? Color.Yellow :
				knowledgeLevel == KnowledgeLevel.None ? Color.Gray : 
				throw new InvalidOperationException();
		}

		public string Id
		{
			get => Flashcard.Id;
			set => Flashcard.Id = value;
		}

		public string LessonId
		{
			get => Flashcard.LessonId;
			set => Flashcard.LessonId = value;
		}

		public string Front
		{
			get => Flashcard.Front;
			set => Flashcard.Front = value;
		}

		public string Back
		{
			get => Flashcard.Back;
			set => Flashcard.Back = value;
		}

		public string ImageUrl
		{
			get => Flashcard.ImageUrl;
			set => Flashcard.ImageUrl = value;
		}

		public DateTime Created
		{
			get => Flashcard.Created;
			set => Flashcard.Created = value;
		}

		public Flashcard Flashcard { get; }
		public Color KnowledgeLevelColor { get; }
	}

	public class FlashcardListViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly IGetFlashcardsKnowledgeLevels _getFlashcardsKnowledgeLevel;
		private readonly INavigationService _navigationService;
		private Lesson _lesson;

		public FlashcardListViewModel(
			IRepository<Flashcard> flashcardRepository,
			INavigationService navigationService,
			IGetFlashcardsKnowledgeLevels getFlashcardsKnowledgeLevel)
		{
			_flashcardRepository = flashcardRepository;
			_navigationService = navigationService;
			_getFlashcardsKnowledgeLevel = getFlashcardsKnowledgeLevel;
		}

		public FlashcardListViewModel()
		{
		}

		public ObservableCollection<FlashcardViewModel> Flashcards { get; } = new ObservableCollection<FlashcardViewModel>();

		public ICommand DeleteFlashcardCommand => new Command<string>(async flashcardId =>
		{
			var flashcardToRemove = Flashcards.Single(f => f.Id == flashcardId);
			await _flashcardRepository.Delete(flashcardToRemove.Flashcard);
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
			var knowledgeLevels = await _getFlashcardsKnowledgeLevel.KnowledgeLevels(_lesson);
			var x = knowledgeLevels.Select(kl => new FlashcardViewModel(kl.Flashcard, kl.KnowledgeLevel));
			Flashcards.SynchronizeWith(x);
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