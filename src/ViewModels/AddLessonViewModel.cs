using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class AddLessonViewModel : INavigatingAware, INotifyPropertyChanged
    {
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly IMessage _message;

		private Lesson _lesson;

		public AddLessonViewModel()
		{
		}

		public AddLessonViewModel(
			INavigationService navigationService,
			IMessage message)
		{
			_navigationService = navigationService;
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
				}
			}
		}

		public bool AskInRepetitions
		{
			get => _lesson?.AskInRepetitions ?? default(bool);
			set => _lesson.AskInRepetitions = value;
		}

		public bool ShuffleFlashcards
		{
			get => _lesson?.Shuffle ?? default(bool);
			set => _lesson.Shuffle = value;
		}

		public IEnumerable<string> AllAskingModes =>
			Enum.GetValues(typeof(AskingMode))
				.Cast<AskingMode>()
				.Select(x => x.Localize())
				.ToList();

		public IList<string> LanguageNames =>
			Enum.GetNames(typeof(Language))
				.ToList();

	    public Language SelectedFrontLanguage
	    {
		    get => _lesson?.FrontLanguage ?? default(Language);
		    set
		    {
			    if (_lesson != null && (int) value != -1)
				    _lesson.FrontLanguage = value;
		    }
	    }

	    public Language SelectedBackLanguage
	    {
		    get => _lesson?.BackLanguage ?? default(Language);
		    set
		    {
			    if (_lesson != null && (int)value != -1)
					_lesson.BackLanguage = value;
		    }
	    }

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
			_lesson = new Lesson
			{
				Id = Guid.NewGuid().ToString(),
				AskInRepetitions = true,
				AskingMode = AskingMode.Front,
				Shuffle = true,
				Flashcards = new List<Flashcard>()
			};

			OnPropertyChanged(nameof(LessonName));
			OnPropertyChanged(nameof(AskingMode));
			OnPropertyChanged(nameof(AskInRepetitions));
			OnPropertyChanged(nameof(ShuffleFlashcards));
		}
	}
}
