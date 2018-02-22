using System;
using System.Collections.Generic;
using System.Windows.Input;
using FlashCards.Services.Database;
using Prism.Navigation;
using Xamarin.Forms;

namespace FlashCards.ViewModels.Lesson
{
    public class LessonListViewModel
    {
        private readonly IRepository<Models.Lesson> _modelsRepository;
        private readonly INavigationService _navigationService;

        public IEnumerable<LessonViewModel> Items; // =

        public LessonListViewModel(IRepository<Models.Lesson> modelsRepository, INavigationService navigationService)
        {
            _modelsRepository = modelsRepository;
            _navigationService = navigationService;
        }

        public ICommand AddLesson => new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });
    }
}