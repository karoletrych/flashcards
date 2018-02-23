using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class AddLessonViewModel
    {
        private readonly AddLessonService _addLessonService;
        private readonly INavigationService _navigationService;

        public AddLessonViewModel(INavigationService navigationService, AddLessonService addLessonService)
        {
            _navigationService = navigationService;
            _addLessonService = addLessonService;
        }

        public IList<string> LanguageNames =>
            Enum.GetNames(typeof(Language))
                .OrderBy(language => language).ToList();

        public string SelectedFrontLanguage { get; set; }
        public string SelectedBackLanguage { get; set; }
        public string LessonName { get; set; }

        public ICommand AddFlashcards => new Command(() =>
        {
            var frontLanguage = SelectedFrontLanguage.ToLanguageEnum();
            var backLanguage = SelectedBackLanguage.ToLanguageEnum();
            var lessonId = _addLessonService.AddLesson(LessonName, frontLanguage, backLanguage);

            _navigationService.NavigateAsync("AddFlashcardPage", new NavigationParameters
            {
                {"frontLanguage", frontLanguage},
                {"backLanguage", backLanguage},
                {"lessonId", lessonId}
            });
        });
    }
}