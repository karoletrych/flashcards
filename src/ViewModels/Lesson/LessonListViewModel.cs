using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Flashcards.Services.Database;
using FlashCards.Services;
using Nito.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class LessonListViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly DeleteLessonService _deleteLessonService;
        private readonly IPageDialogService _dialogService;
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;

        public LessonListViewModel(
            IRepository<Models.Lesson> lessonRepository,
            INavigationService navigationService,
            IPageDialogService dialogService,
            DeleteLessonService deleteLessonService)
        {
            _lessonRepository = lessonRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _deleteLessonService = deleteLessonService;
            Lessons = new ObservableCollection<Models.Lesson>();
        }

        public ObservableCollection<Models.Lesson> Lessons { get; }

        public ICommand AddLessonCommand =>
            new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

        public ICommand PracticeLessonCommand => new Command<Models.Lesson>(lesson =>
        {
            DialogHandler.HandleExceptions(_dialogService, async () =>
            {
                var lessonId = new NavigationParameters {{"lessonId", lesson.Id}};
                await _navigationService.NavigateAsync("AskingQuestionsPage", lessonId);
            });
        });

        public ICommand EditLessonCommand =>
            new Command<Models.Lesson>(lesson =>
            {
                _dialogService.DisplayAlertAsync("Edycja lekcji jeszcze nie gotowa", "", "OK, spoko");
            });

        public ICommand DeleteLessonCommand =>
            new Command<Models.Lesson>(
                async lesson =>
                {
                    await _deleteLessonService.Delete(lesson.Id);
                    Lessons.Remove(lesson);
                });

        public event PropertyChangedEventHandler PropertyChanged;

        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            var lessons = await _lessonRepository.FindAll();
            foreach (var lesson in lessons)
            {
                Lessons.Add(lesson);
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}