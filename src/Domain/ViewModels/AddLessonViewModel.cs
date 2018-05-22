using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Infrastructure.Localization;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Models;
using Prism.Commands;
using Prism.Navigation;

namespace Flashcards.Domain.ViewModels
{
    public class AddLessonViewModel : INavigatingAware, INotifyPropertyChanged
    {
	    private readonly INavigationService _navigationService;
		private readonly IMessage _message;


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

	    public string LessonName { get; set; }

	    public AskingMode AskingMode { get; set; }

		public bool AskInRepetitions { get; set; }

		public bool ShuffleFlashcards { get; set; }

		public IEnumerable<string> AllAskingModes =>
			Enum.GetValues(typeof(AskingMode))
				.Cast<AskingMode>()
				.Select(x => x.Localize())
				.ToList();

		public IList<string> LanguageNames =>
			Enum.GetNames(typeof(Language))
				.ToList();

	    public Language SelectedFrontLanguage { get; set; }

	    public Language SelectedBackLanguage { get; set; }


	    public bool CanNavigate { get; set; } = true;
		public ICommand FlashcardListCommand => new DelegateCommand(FlashcardList).ObservesCanExecute(() => CanNavigate);

	    private async void FlashcardList()
	    {
		    if (string.IsNullOrWhiteSpace(LessonName))
		    {
			    _message.LongAlert(AppResources.InsertLessonName);
			    return;
		    }

		    CanNavigate = false;

		    var lesson = Lesson.Create(SelectedFrontLanguage, SelectedBackLanguage);
		    lesson.AskInRepetitions = AskInRepetitions;
		    lesson.AskingMode = AskingMode;
		    lesson.Name = LessonName;
		    lesson.Shuffle = ShuffleFlashcards;

			await _navigationService.NavigateAsync(
			    "FlashcardListPage",
			    new NavigationParameters
			    {
				    {
					    "lesson", lesson
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
			AskInRepetitions = true;
			AskingMode = AskingMode.Front;
		}
	}
}
