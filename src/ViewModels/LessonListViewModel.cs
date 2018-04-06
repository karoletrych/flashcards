using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Localization;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
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
		private readonly IRepetitor _repetitor;
		private readonly IRepetitionFlashcardsRetriever _repetitionFlashcardsRetriever;
		private readonly ISpacedRepetition _spacedRepetition;

		private ICollection<Flashcard> PendingRepetitionFlashcards { get; set; } = new List<Flashcard>();

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
			IRepetitor repetitor,
			IRepetitionFlashcardsRetriever repetitionFlashcardsRetriever)
		{
			_lessonRepository = lessonRepository;
			_navigationService = navigationService;
			_dialogService = dialogService;
			_flashcardRepository = flashcardRepository;
			_examinerBuilder = examinerBuilder;
			_spacedRepetition = spacedRepetition;
			_repetitor = repetitor;
			_repetitionFlashcardsRetriever = repetitionFlashcardsRetriever;
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			var lessons = (await _lessonRepository.FindAll()).ToList();

			UpdateRepetitionDisplay();
			UpdateLessons();

			async void UpdateRepetitionDisplay()
			{
				PendingRepetitionFlashcards = await _repetitionFlashcardsRetriever.FlashcardsToAsk();
				PendingRepetitionFlashcardsNumber = PendingRepetitionFlashcards.Count();

				var learnedFlashcards = lessons
					.Where(l => l.AskInRepetitions)
					.SelectMany(l => l.Flashcards)
					.Intersect(_spacedRepetition.LearnedFlashcards)
					.Count();

				var totalActiveFlashcards = lessons
					.Where(l => l.AskInRepetitions)
					.SelectMany(l => l.Flashcards)
					.Count();

				ActiveRepetitionsRatio = (double) learnedFlashcards / totalActiveFlashcards;
				ActiveRepetitionsRatioString = learnedFlashcards + "/" + totalActiveFlashcards;
			}

			void UpdateLessons()
			{
				Lessons.Clear();
				foreach (var lesson in lessons)
					Lessons.Add(new LessonViewModel(lesson, _spacedRepetition.LearnedFlashcards));
			}
		}

		public ObservableCollection<LessonViewModel> Lessons { get; } = new ObservableCollection<LessonViewModel>();

		public ICommand AddLessonCommand =>
			new Command(() => { _navigationService.NavigateAsync("AddLessonPage"); });

		public ICommand PracticeLessonCommand => new Command<LessonViewModel>(async lesson =>
		{
			var flashcards = await _flashcardRepository.Where(f => f.LessonId == lesson.InternalLesson.Id);
			var examiner = _examinerBuilder
				.WithFlashcards(flashcards)
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

		public int PendingRepetitionFlashcardsNumber { get; private set; }

		public string ActiveRepetitionsRatioString { get; private set; }

		public double ActiveRepetitionsRatio { get; private set; }

		public ICommand RunRepetitionCommand =>
			new Command(async () =>
				{
					if(PendingRepetitionFlashcards.Any())
						await _repetitor.Repeat(_navigationService, "AskingQuestionsPage", PendingRepetitionFlashcards);
					else
						await _dialogService.DisplayAlertAsync(
							AppResources.NoFlashcards, 
							AppResources.NoRepetitionFlashcardsForToday, 
							"OK");
				}
			);

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}


#pragma warning disable 0067
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

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