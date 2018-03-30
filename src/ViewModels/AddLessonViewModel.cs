using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Flashcards.Localization;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AddLessonViewModel
	{
		private readonly IMessage _message;
		private readonly INavigationService _navigationService;

		public AddLessonViewModel()
		{
		}

		public AddLessonViewModel(
			INavigationService navigationService,
			IMessage message)
		{
			(_navigationService, _message) =
				(navigationService, message);
		}

		public IList<string> LanguageNames =>
			Enum.GetNames(typeof(Language))
				.OrderBy(language => language).ToList();


		public string SelectedFrontLanguage { get; set; }
		public string SelectedBackLanguage { get; set; }
		public string LessonName { get; set; }
		public IList<string> AllAskingModes => Enum.GetNames(typeof(AskingMode));
		public bool AskInRepetitions { get; set; }
		public AskingMode AskingMode { get; set; }


		public ICommand AddFlashcards => new Command(async () =>
		{
			if (string.IsNullOrWhiteSpace(LessonName))
			{
				_message.ShortAlert(Localization.AppResources.InsertLessonName);
				return;
			}

			if (string.IsNullOrWhiteSpace(SelectedFrontLanguage))
			{
				_message.ShortAlert(AppResources.ChooseFrontLanguage);
				return;
			}

			if (string.IsNullOrWhiteSpace(SelectedBackLanguage))
			{
				_message.ShortAlert(AppResources.ChooseBackLanguage);
				return;
			}

			var frontLanguage = SelectedFrontLanguage.ToLanguageEnum();
			var backLanguage = SelectedBackLanguage.ToLanguageEnum();
			var lessonId = Guid.NewGuid().ToString();
			var lesson = new Lesson
			{
				Id = lessonId,
				Name = LessonName,
				BackLanguage = backLanguage,
				FrontLanguage = frontLanguage,
				AskingMode = AskingMode,
				AskInRepetitions = AskInRepetitions
			};

			await _navigationService.NavigateAsync("AddFlashcardPage",
				new NavigationParameters
				{
					{
						"lesson", lesson
					}
				});
		});
	}
}