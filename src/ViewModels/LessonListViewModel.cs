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
		private readonly ExaminerBuilder _examinerBuilder;
		private readonly IRepository<Flashcard> _flashcardRepository;
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly IRepetition _repetition;
		private readonly IRepetitionFlashcardsRetriever _repetitionFlashcardsRetriever;
		private readonly ISpacedRepetition _spacedRepetition;

		private int _learnedFlashcards;

		private IEnumerable<Flashcard> _pendingRepetitionFlashcards = new List<Flashcard>();
		private int _totalActiveFlashcards;

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
			IRepetitionFlashcardsRetriever repetitionFlashcardsRetriever)
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

		public ICommand PracticeLessonCommand => new Command<LessonViewModel>(async lesson =>
		{
			var flashcards = await _flashcardRepository.Where(f => f.LessonId == lesson.InternalLesson.Id);
			var examiner = _examinerBuilder
				.WithFlashcards(flashcards)
				.WithRepeatingQuestions(true)
				.WithAskingMode(lesson.InternalLesson.AskingMode)
				.Build();

			await _navigationService.NavigateAsync("AskingQuestionsPage", new NavigationParameters
			{
				{
					"examiner", examiner
				}
			});
		});

		public ICommand EditLessonCommand =>
			new Command<LessonViewModel>(async lesson =>
			{
				await _navigationService.NavigateAsync("EditLessonPage", new NavigationParameters
				{
					{
						"lessonId", lesson.InternalLesson.Id
					}
				});
			});

		public ICommand SettingsCommand
		{
			get { return new Command(async () => { await _navigationService.NavigateAsync("SettingsPage"); }); }
		}

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
					catch (InvalidOperationException)
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
				lessons
					.Where(l => l.AskInRepetitions).SelectMany(l => l.Flashcards)
					.Intersect(_spacedRepetition.LearnedFlashcards)
					.Count();

			_totalActiveFlashcards = 
				lessons.SelectMany(l => l.Flashcards).Count();
			OnPropertyChanged(nameof(ActiveRepetitionsRatio));
			OnPropertyChanged(nameof(ActiveRepetitionsRatioString));

			Lessons.Clear();
			foreach (var lesson in lessons)
				Lessons.Add(new LessonViewModel(lesson, _spacedRepetition.LearnedFlashcards));
		}


#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
		}

		public class LessonViewModel
		{
			public LessonViewModel(Lesson internalLesson, IEnumerable<Flashcard> learnedFlashcards)
			{
				var lessonFlashcardsCount = internalLesson.Flashcards.Count;
				var learnedFlashcardsCount = internalLesson.Flashcards.Intersect(learnedFlashcards).Count();

				FrontLanguage = internalLesson.FrontLanguage;
				BackLanguage = internalLesson.BackLanguage;
				Name = internalLesson.Name;
				LearnedFlashcardsRatioString = learnedFlashcardsCount + "/" + lessonFlashcardsCount;
				LearnedFlashcardsRatio = (double) learnedFlashcardsCount / lessonFlashcardsCount;
				InternalLesson = internalLesson;
			}

			public string Name { get; }
			public Language BackLanguage { get; }
			public Language FrontLanguage { get; }
			public string LearnedFlashcardsRatioString { get; }
			public double LearnedFlashcardsRatio { get; }
			public Lesson InternalLesson { get; }
		}
	}
}