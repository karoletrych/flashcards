using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels.Lesson
{
    public class LessonListViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly IPageDialogService _dialogService;
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Models.Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly Func<IEnumerable<Flashcard>, Examiner> _examinerFactory;

        public LessonListViewModel()
        {
            
        }

        public LessonListViewModel(
            IRepository<Models.Lesson> lessonRepository,
            INavigationService navigationService,
            IPageDialogService dialogService,
            IRepository<Flashcard> flashcardRepository,
            Func<IEnumerable<Flashcard>, Examiner> examinerFactory)
        {
            _lessonRepository = lessonRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _flashcardRepository = flashcardRepository;
            _examinerFactory = examinerFactory;
            Lessons = new ObservableCollection<Models.Lesson>();
        }

        public ObservableCollection<Models.Lesson> Lessons { get; }

        public ICommand AddLessonCommand =>
            new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

        public ICommand PracticeLessonCommand => new Command<Models.Lesson>(lesson =>
        {
            DialogHandler.HandleExceptions(_dialogService, async () =>
            {
                var flashcards = await _flashcardRepository.FindMatching(f => f.LessonId == lesson.Id);
                var examiner = _examinerFactory(flashcards);

                await _navigationService.NavigateAsync("AskingQuestionsPage", new NavigationParameters
                {
                    {
                        "examiner", examiner
                    }
                });
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
                    await _lessonRepository.Delete(lesson);
                    Lessons.Remove(lesson);
                });

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
	        Lessons.Clear();
			var lessons = await _lessonRepository.FindAll();
            foreach (var lesson in lessons) Lessons.Add(lesson);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}