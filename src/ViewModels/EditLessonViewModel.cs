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
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly IPageDialogService _dialogService;

		private Lesson _lesson;

		public EditLessonViewModel()
		{
		}

		public EditLessonViewModel(
			INavigationService navigationService, 
			IRepository<Lesson> lessonRepository, 
			IRepository<Flashcard> flashcardRepository,
			IPageDialogService dialogService)
		{
			_navigationService = navigationService;
			_lessonRepository = lessonRepository;
			_flashcardRepository = flashcardRepository;
			_dialogService = dialogService;
		}

		public ObservableCollection<Flashcard> Flashcards { get; set; } =
			new ObservableCollection<Flashcard>();

		public ICommand AddFlashcardsCommand => new Command(() =>
			_navigationService.NavigateAsync("AddFlashcardPage", new NavigationParameters
			{
				{
					"lesson", _lesson
				}
			}));

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


		public IList<string> AllAskingModes => Enum.GetNames(typeof(AskingMode));

		public ICommand DeleteFlashcardCommand => new Command<int>(async flashcardId =>
		{
			var flashcardToRemove = new Flashcard{Id = flashcardId};
			await _flashcardRepository.Delete(flashcardToRemove);
			Flashcards.Remove(flashcardToRemove);
		});

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
			_lesson = (_lessonRepository.Where(lesson => lesson.Id == lessonId).Result).Single();

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