using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class LessonListViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly IPageDialogService _dialogService;
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly Func<IEnumerable<Flashcard>, Examiner> _examinerFactory;

	    public LessonListViewModel()
        {
        }

	    public LessonListViewModel(
            IRepository<Lesson> lessonRepository,
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
        }

	    public ObservableCollection<Lesson> Lessons { get; } = new ObservableCollection<Lesson>();

		public ICommand AddLessonCommand =>
            new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

	    public ICommand PracticeLessonCommand => new Command<Lesson>(lesson =>
        {
            ExceptionHandler.HandleWithDialog(_dialogService, async () =>
            {
                var flashcards = await _flashcardRepository.Where(f => f.LessonId == lesson.Id);
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
            new Command<Lesson>(async lesson =>
            {
	            await _navigationService.NavigateAsync("EditLessonPage", new NavigationParameters
	            {
		            {
			            "lessonId", lesson.Id
		            }
	            });
            });

	    public ICommand DeleteLessonCommand =>
            new Command<Lesson>(
                async lesson =>
                {
                    await _lessonRepository.Delete(lesson);
                    Lessons.Remove(lesson);
                });

	    public ICommand SettingsCommand
	    {
		    get
		    {
			    return new Command(async () =>
			    {
				    await _navigationService.NavigateAsync("SettingsPage");
			    });
		    }
	    }

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
#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

	}
}