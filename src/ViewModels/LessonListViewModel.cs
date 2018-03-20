using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class LessonListViewModel : INotifyPropertyChanged, INavigatedAware
    {
        private readonly IPageDialogService _dialogService;
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly ExaminerBuilder _examinerBuilder;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly INavigationService _navigationService;
        private readonly ISpacedRepetition _spacedRepetition;
        private readonly IRepetition _repetition;
        private readonly RepetitionFlashcardsRetriever _repetitionFlashcardsRetriever;

        public LessonListViewModel()
        {
        }

        public LessonListViewModel(
            IRepository<Lesson> lessonRepository,
            INavigationService navigationService,
            IPageDialogService dialogService,
            IRepository<Flashcard> flashcardRepository,
            ExaminerBuilder examinerBuilder,
            ISpacedRepetition spacedRepetition,
			IRepetition repetition,
            RepetitionFlashcardsRetriever repetitionFlashcardsRetriever)
        {
            _lessonRepository = lessonRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _flashcardRepository = flashcardRepository;
            _examinerBuilder = examinerBuilder;
            _spacedRepetition = spacedRepetition;
	        _repetition = repetition;
	        _repetitionFlashcardsRetriever = repetitionFlashcardsRetriever;
        }

        public ObservableCollection<LessonViewModel> Lessons { get; } = new ObservableCollection<LessonViewModel>();

        public ICommand AddLessonCommand =>
            new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

        public ICommand PracticeLessonCommand => new Command<Lesson>(lesson =>
        {
            ExceptionHandler.HandleWithDialog(_dialogService, async () =>
            {
                var flashcards = await _flashcardRepository.Where(f => f.LessonId == lesson.Id);
                var examiner = _examinerBuilder.WithFlashcards(flashcards).Build();

                await _navigationService.NavigateAsync("AskingQuestionsPage", new NavigationParameters
                {
                    {
                        "examiner", examiner
                    }
                });
            });
        });

        public ICommand EditLessonCommand =>
            new Command<LessonViewModel>(async lesson =>
            {
                await _navigationService.NavigateAsync("EditLessonPage", new NavigationParameters
                {
                    {
                        "lessonId", lesson.Id
                    }
                });
            });

        public ICommand DeleteLessonCommand =>
            new Command<LessonViewModel>(
                async lesson =>
                {
                    var lessonViewModel = Lessons.Single(l => l.Id == lesson.Id);
                    await _lessonRepository.Delete(lessonViewModel.Lesson);
                    Lessons.Remove(lessonViewModel);
                });

        public ICommand SettingsCommand
        {
            get { return new Command(async () => { await _navigationService.NavigateAsync("SettingsPage"); }); }
        }

        private int _learnedFlashcards;
        private int _totalActiveFlashcards;

	    private IEnumerable<Flashcard> _pendingRepetitionFlashcards = new List<Flashcard>();
	    public int PendingRepetitionFlashcardsNumber => _pendingRepetitionFlashcards.Count();
		public string ActiveRepetitionsRatioString => _learnedFlashcards + "/" + _totalActiveFlashcards;
        public double ActiveRepetitionsRatio => (double) _learnedFlashcards / _totalActiveFlashcards;

	    public ICommand RunRepetitionCommand =>
		    new Command(async () =>
			    {
				    try
				    {
					    await _repetition.Repeat(_navigationService, _pendingRepetitionFlashcards);
				    }
				    catch(InvalidOperationException)
				    {
					    await _dialogService.DisplayAlertAsync("No flashcards", "No repetition flashcards for today", "OK");
				    }
			    }
		    );

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
	        _pendingRepetitionFlashcards = await _repetitionFlashcardsRetriever.FlashcardsToAsk();
	        OnPropertyChanged(nameof(PendingRepetitionFlashcardsNumber));


			var lessons = (await _lessonRepository.FindAll()).ToList();

	        _learnedFlashcards =
		        _totalActiveFlashcards =
			        lessons.Where(l => l.AskInRepetitions).SelectMany(l => l.Flashcards).Count();
	        _totalActiveFlashcards = lessons.SelectMany(l => l.Flashcards).Count();
			OnPropertyChanged(nameof(ActiveRepetitionsRatio));
			OnPropertyChanged(nameof(ActiveRepetitionsRatioString));

            Lessons.Clear();
            foreach (var lesson in lessons)
                Lessons.Add(new LessonViewModel(lesson, _spacedRepetition.LearnedFlashcards));
        }

	    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
	    }


#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        public class LessonViewModel
        {
            public LessonViewModel(Lesson lesson, IEnumerable<Flashcard> learnedFlashcards)
            {
                var lessonFlashcardsCount = lesson.Flashcards.Count;
                var learnedFlashcardsCount = lesson.Flashcards.Intersect(learnedFlashcards).Count();

                Id = lesson.Id;
                FrontLanguage = lesson.FrontLanguage;
                BackLanguage = lesson.BackLanguage;
                Name = lesson.Name;
                LearnedFlashcardsRatioString = learnedFlashcardsCount + "/" + lessonFlashcardsCount;
                LearnedFlashcardsRatio = (double) learnedFlashcardsCount / lessonFlashcardsCount;
                Lesson = lesson;
            }

            public string Id { get; }
            public string Name { get; }
            public Language BackLanguage { get; }
            public Language FrontLanguage { get; }
            public string LearnedFlashcardsRatioString { get; }
            public double LearnedFlashcardsRatio { get; }
            public Lesson Lesson { get; }
        }
    }
}