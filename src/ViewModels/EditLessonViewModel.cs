using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Localization;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Services.DataAccess;
using Flashcards.ViewModels.Tools;
using Prism.Commands;
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
		private readonly IMessage _message;

		public EditLessonViewModel()
		{
		}

		public EditLessonViewModel(
			INavigationService navigationService, 
			IRepository<Lesson> lessonRepository, 
			IPageDialogService dialogService,
			IMessage message)
		{
			_navigationService = navigationService;
			_lessonRepository = lessonRepository;
			_dialogService = dialogService;
			_message = message;
		}

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
				if (_lesson == null || (int) value == -1) return;
				_lesson.AskingMode = value;
				_lessonRepository.Update(_lesson);
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
				.Select(x => x.Localize())
				.ToList();



		public ICommand DeleteLessonCommand => new Command(async () =>
		{
			var sure = await _dialogService.DisplayAlertAsync(
				AppResources.Warning, 
				AppResources.AreYouSure, 
				AppResources.Yes, 
				AppResources.No);
			if (sure)
			{
				await _lessonRepository.DeleteWithChildren(_lesson);
				await _navigationService.GoBackAsync();
			}
		});

		public bool CanNavigate { get; set; } = true;

		public ICommand FlashcardListCommand => 
			new DelegateCommand(FlashcardList)
			.ObservesCanExecute(() => CanNavigate);

		private void FlashcardList()
		{
			if (string.IsNullOrWhiteSpace(_lesson.Name))
			{
				_message.LongAlert(AppResources.InsertLessonName);
				return;
			}

			CanNavigate = false;
			_navigationService.NavigateAsync(
				"FlashcardListPage",
				new NavigationParameters
				{
					{
						"lesson", _lesson
					}
				});
			CanNavigate = true;
		}


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
			_lesson = _lessonRepository.GetWithChildren(lesson => lesson.Id == lessonId).Result.Single();

			OnPropertyChanged(nameof(LessonName));
			OnPropertyChanged(nameof(AskingMode));
			OnPropertyChanged(nameof(AskInRepetitions));
			OnPropertyChanged(nameof(ShuffleFlashcards));
		}
	}
}