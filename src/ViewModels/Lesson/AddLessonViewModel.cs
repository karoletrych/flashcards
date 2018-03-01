using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class AddLessonViewModel
    {
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;

        public AddLessonViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IRepository<Models.Lesson> lessonRepository) =>
            (_navigationService, _pageDialogService, _lessonRepository) = 
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

        public ICommand AddFlashcards => new Command(() =>
            DialogHandler.HandleExceptions(_pageDialogService, async () =>
            {
                var frontLanguage = SelectedFrontLanguage.ToLanguageEnum();
                var backLanguage = SelectedBackLanguage.ToLanguageEnum();

                var lesson = new Models.Lesson
                {
                    Name = LessonName,
                    BackLanguage = backLanguage,
                    FrontLanguage = frontLanguage
                };
                var lessonId = await _lessonRepository.Insert(lesson);

                await _navigationService.NavigateAsync("AddFlashcardPage", new NavigationParameters
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
            })
        );
    }
}