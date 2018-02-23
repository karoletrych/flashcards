using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Flashcards.Services.Database;
using Nito.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class LessonListViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        public LessonListViewModel(
            IRepository<Models.Lesson> lessonRepository,
            INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _lessonRepository = lessonRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        public NotifyTask<IEnumerable<Models.Lesson>> Items =>
            NotifyTask.Create(async () => await _lessonRepository.FindAll());

        public ICommand AddLesson =>
            new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

        public ICommand LessonTappedCommand => new Command<Models.Lesson>(async lesson =>
        {
            HandleExceptions(_dialogService, async () =>
            {
                var lessonId = new NavigationParameters { { "lessonId", lesson.Id } };
                await _navigationService.NavigateAsync("AskingQuestionsPage", lessonId);
            });
        });

        private async void HandleExceptions(IPageDialogService dialogService, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                await dialogService.DisplayAlertAsync("Something is no yes", e.ToString(), "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}