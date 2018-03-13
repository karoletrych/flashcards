using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class AddLessonViewModel
    {
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

	    public AddLessonViewModel()
	    {
		    
	    }

        public AddLessonViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IRepository<Models.Lesson> lessonRepository) =>
            (_navigationService, _dialogService, _lessonRepository) = 
                (navigationService, pageDialogService, lessonRepository);

        public IList<string> FrontLanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public IList<string> BackLanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public string SelectedFrontLanguage { get; set; }
        public string SelectedBackLanguage { get; set; }
        public string LessonName { get; set; }

	    public ICommand AddFlashcards => new Command(async () =>
	    {
		    var frontLanguage = SelectedFrontLanguage.ToLanguageEnum();
		    var backLanguage = SelectedBackLanguage.ToLanguageEnum();
		    var lessonId = Guid.NewGuid().ToString();
		    if (string.IsNullOrWhiteSpace(LessonName))
		    {
			    await _dialogService.DisplayAlertAsync("Błąd", "Nazwa lekcji nie może być pusta", "OK");
				return;
		    }
		    if (string.IsNullOrWhiteSpace(SelectedFrontLanguage))
		    {
			    await _dialogService.DisplayAlertAsync("Błąd", "Wybierz język frontu fiszek", "OK");
			    return;
			}
		    if (string.IsNullOrWhiteSpace(SelectedBackLanguage))
		    {
			    await _dialogService.DisplayAlertAsync("Błąd", "Wybierz język tyłu fiszek", "OK");
			    return;
			}

			var lesson = new Models.Lesson
		    {
			    Id = lessonId,
			    Name = LessonName,
			    BackLanguage = backLanguage,
			    FrontLanguage = frontLanguage
		    };
		    await _lessonRepository.Insert(lesson);

		    await _navigationService.NavigateAsync("AddFlashcardPage",
			    new NavigationParameters
			    {
				    {
					    "frontLanguage", frontLanguage
				    },
				    {
					    "backLanguage", backLanguage
				    },
				    {
					    "lessonId", lessonId
				    }
			    });
	    });
    }
}