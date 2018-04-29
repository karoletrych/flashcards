using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Flashcards.Domain.ViewModels.Tools;
using Flashcards.Infrastructure.Localization;
using Flashcards.Infrastructure.Settings;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Examiner;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.Domain.ViewModels
{
	public class RepetitionViewModel : INavigatedAware, INotifyPropertyChanged
	{
		private readonly IRepetitionExaminerBuilder _repetitionExaminerBuilder;
		private readonly IRepository<Lesson> _lessonRepository;
		private readonly ISetting<int> _streakDaysSetting;
		private readonly ISpacedRepetition _spacedRepetition;
		private readonly IRepetitor _repetitor;
		private readonly INavigationService _navigationService;
		private readonly IPageDialogService _dialogService;

		public RepetitionViewModel(
			ISetting<int> streakDaysSetting,
			IRepetitionExaminerBuilder repetitionExaminerBuilder,
			IRepository<Lesson> lessonRepository,
			ISpacedRepetition spacedRepetition,
			IRepetitor repetitor,
			INavigationService navigationService,
			IPageDialogService dialogService)
		{
			_streakDaysSetting = streakDaysSetting;
			_repetitionExaminerBuilder = repetitionExaminerBuilder;
			_lessonRepository = lessonRepository;
			_spacedRepetition = spacedRepetition;
			_repetitor = repetitor;
			_navigationService = navigationService;
			_dialogService = dialogService;
		}

		public bool NoPendingRepetitions => PendingRepetitionQuestionsNumber == 0;
		public string ActiveRepetitionsRatioString { get; private set; }
		public double ActiveRepetitionsRatio { get; private set; }
		public int RepetitionStreakDays { get; private set; }
		public int PendingRepetitionQuestionsNumber { get; private set; }
		private IExaminer PendingRepetitionExaminer { get; set; }


		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}


		public ICommand RunRepetitionCommand =>
			new Command(async () =>
				{
					if (PendingRepetitionQuestionsNumber > 0)
						await _repetitor.Repeat(_navigationService, "AskingQuestionsPage", PendingRepetitionExaminer);
					else
						await _dialogService.DisplayAlertAsync(
							AppResources.NoFlashcards,
							AppResources.NoRepetitionFlashcardsForToday,
							"OK");
				}
			);


		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			PendingRepetitionExaminer = await _repetitionExaminerBuilder.BuildExaminer();
			PendingRepetitionQuestionsNumber = PendingRepetitionExaminer.QuestionsCount;

			var activeLessons = (await _lessonRepository.GetWithChildren(l => l.AskInRepetitions)).ToList();
			var activeFlashcards = activeLessons.SelectMany(l => l.Flashcards).ToList();

			var learnedFlashcardsCount = activeFlashcards
				.Intersect(await _spacedRepetition.LearnedFlashcards())
				.Count();

			var totalActiveFlashcardsCount = activeFlashcards.Count;

			ActiveRepetitionsRatio = (double) learnedFlashcardsCount / totalActiveFlashcardsCount;
			ActiveRepetitionsRatioString = learnedFlashcardsCount + "/" + totalActiveFlashcardsCount;
			RepetitionStreakDays = _streakDaysSetting.Value;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}