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
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class LessonListViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IPageDialogService _dialogService;
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly INavigationService _navigationService;
		private readonly IRepetitor _repetitor;
		private readonly IRepetitionExaminerBuilder _repetitionExaminerBuilder;
		private readonly ISpacedRepetition _spacedRepetition;
		private readonly ISetting<int> _streakDaysSetting;

		private IExaminer PendingRepetitionExaminer { get; set; }
		public int PendingRepetitionQuestionsNumber { get; private set; }
		public bool NoPendingRepetitions => PendingRepetitionQuestionsNumber == 0;
		public string ActiveRepetitionsRatioString { get; private set; }
		public double ActiveRepetitionsRatio { get; private set; }
		public int RepetitionStreakDays { get; private set; }

		public LessonListViewModel()
		{
		}

		public LessonListViewModel(
			IRepository<Lesson> lessonRepository,
			INavigationService navigationService,
			IPageDialogService dialogService,
			ISpacedRepetition spacedRepetition,
			IRepetitor repetitor,
			IRepetitionExaminerBuilder repetitionExaminerBuilder,
			ISetting<int> streakDaysSetting)
		{
			_lessonRepository = lessonRepository;
			_navigationService = navigationService;
			_dialogService = dialogService;
			_spacedRepetition = spacedRepetition;
			_repetitor = repetitor;
			_repetitionExaminerBuilder = repetitionExaminerBuilder;
			_streakDaysSetting = streakDaysSetting;
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			var lessons = (await _lessonRepository.FindAll()).ToList();

			UpdateRepetitionDisplay();
			UpdateLessons();

			async void UpdateRepetitionDisplay()
			{
				PendingRepetitionExaminer = (await _repetitionExaminerBuilder.Examiner());
				PendingRepetitionQuestionsNumber = PendingRepetitionExaminer.QuestionsCount;

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
				RepetitionStreakDays = _streakDaysSetting.Value;
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
			var examiner = new ExaminerBuilder()
				.WithLessons(new []{lesson.InternalLesson})
				.WithAskingMode(lesson.InternalLesson.AskingMode)
				.WithShuffling(lesson.InternalLesson.Shuffle)
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



		public ICommand RunRepetitionCommand =>
			new Command(async () =>
				{
					if(PendingRepetitionQuestionsNumber > 0)
						await _repetitor.Repeat(_navigationService, "AskingQuestionsPage", PendingRepetitionExaminer);
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