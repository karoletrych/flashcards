using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class EditLessonViewModel : INavigatingAware, INotifyPropertyChanged
	{
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly IPageDialogService _dialogService;

		private Lesson _lesson;

		public EditLessonViewModel()
		{
		}

		public EditLessonViewModel(
			INavigationService navigationService, 
			IRepository<Lesson> lessonRepository, 
			IPageDialogService dialogService)
		{
			_navigationService = navigationService;
			_lessonRepository = lessonRepository;
			_dialogService = dialogService;
		}

		public ObservableCollection<Flashcard> Flashcards { get; set; } =
			new ObservableCollection<Flashcard>();

		public string LessonName
		{
			get => _lesson?.Name ?? string.Empty;
			set
			{
				if (_lesson != null)
				{
					_lesson.Name = value;
					_lessonRepository.Update(_lesson);
				}
			}
		}

		public AskingMode AskingMode
		{
			get => _lesson?.AskingMode ?? default(AskingMode);
			set
			{
				if (_lesson != null && (int)value != -1) // prevents xamarin from setting default value when navigating back
				{
					_lesson.AskingMode = value;
					_lessonRepository.Update(_lesson);
				}
			}
		}

		public bool AskInRepetitions
		{
			get => _lesson?.AskInRepetitions ?? default(bool);
			set
			{
				_lesson.AskInRepetitions = value;
				_lessonRepository.Update(_lesson);
			}
		}

		public bool ShuffleFlashcards
		{
			get => _lesson?.Shuffle ?? default(bool);
			set
			{
				_lesson.Shuffle = value;
				_lessonRepository.Update(_lesson);
			}
		}


		public IEnumerable<string> AllAskingModes => 
			Enum.GetValues(typeof(AskingMode))
				.Cast<AskingMode>()
				.Select(x=>x.Localize())
				.ToList();



		public ICommand DeleteLessonCommand => new Command(async () =>
		{
			var sure = await _dialogService.DisplayAlertAsync(
				Localization.AppResources.Warning, 
				Localization.AppResources.AreYouSure, 
				Localization.AppResources.Yes, 
				Localization.AppResources.No);
			if (sure)
			{
				await _lessonRepository.Delete(_lesson);
				await _navigationService.GoBackAsync();
			}
		});

		public IList<string> LanguageNames => 
			Enum.GetNames(typeof(Language))
				.OrderBy(language => language)
				.ToList();

		public Language SelectedFrontLanguage { get; set; }
		public Language SelectedBackLanguage { get; set; }

		public ICommand FlashcardListCommand => new Command(() =>
			_navigationService.NavigateAsync(
				"FlashcardListPage",
				new NavigationParameters
				{
					{"lesson", _lesson}
				}));


		public void OnNavigatedTo(NavigationParameters parameters)
		{
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
		public void OnNavigatingTo(NavigationParameters parameters)
		{
			var lessonId = (string)parameters["lessonId"];
			if (lessonId == null)
			{
				_lesson = new Lesson
				{
					Id = Guid.NewGuid().ToString(),
					AskInRepetitions = true,
					AskingMode = AskingMode.Front,
					Shuffle = true,
				};
			}
			else
			{
				_lesson = _lessonRepository.FindWhere(lesson => lesson.Id == lessonId).Result.Single();
			}

			OnPropertyChanged(nameof(LessonName));
			OnPropertyChanged(nameof(AskingMode));
			OnPropertyChanged(nameof(AskInRepetitions));
			OnPropertyChanged(nameof(ShuffleFlashcards));

			Flashcards.Clear();
			foreach (var flashcard in _lesson.Flashcards)
				Flashcards.Add(flashcard);
		}
	}
}