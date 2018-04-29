using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Flashcards.Infrastructure.Localization;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Flashcards.ViewModels.Tools;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly Func<IExaminer, CorrectAnswersProgressCalculator> _correctAnswerRatioTrackerFactory;

		private readonly IPageDialogService _dialogService;

		private readonly INavigationService _navigationService;
		private readonly ITextToSpeech _textToSpeech;

		private bool _backIsVisible;
		private Language _backLanguage;
		private bool _canAnswer = true;
		private CorrectAnswersProgressCalculator _correctAnswersProgressCalculator;

		private IExaminer _examiner;
		private Language _frontLanguage;

		public AskingQuestionsViewModel(
			INavigationService navigationService,
			IPageDialogService dialogService,
			ITextToSpeech textToSpeech,
			Func<IExaminer, CorrectAnswersProgressCalculator> correctAnswerRatioTrackerFactory)
		{
			_navigationService = navigationService;
			_dialogService = dialogService;
			_textToSpeech = textToSpeech;
			_correctAnswerRatioTrackerFactory = correctAnswerRatioTrackerFactory;
		}

		public AskingQuestionsViewModel()
		{
		}

		public int CurrentQuestionNumber { get; private set; } = 0;

		public bool FrontIsVisible => !BackIsVisible;

		public string FrontText { get; private set; }
		public bool Revealed { get; private set; }
		public bool NotRevealed => !Revealed;


		public string BackText { get; private set; }

		public ObservableCollection<MulticolorbarItem> QuestionStatuses { get; } =
			new ObservableCollection<MulticolorbarItem>();

		public Uri ImageUri { get; set; }

		public bool BackIsVisible
		{
			get => _backIsVisible;
			private set
			{
				if (_backIsVisible == value)
					return;
				_backIsVisible = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: nameof(FrontIsVisible)));
			}
		}

		public ICommand UserAnswerCommand => new Command<bool>(known =>
		{
			QuestionStatuses[CurrentQuestionNumber] =
				known
					? new MulticolorbarItem
					{
						Color = Color.LawnGreen,
						Value = 1
					}
					: new MulticolorbarItem
					{
						Color = Color.Red,
						Value = 1
					};

			BackIsVisible = false;

			++CurrentQuestionNumber;

			_examiner.Answer(known: known);
			TryShowNextQuestion();
		}, b => _canAnswer);

		public ICommand ShowBackCommand => new Command(() =>
		{
			BackIsVisible = true;
			Revealed = true;
		});

		public string QuestionsProgress => CurrentQuestionNumber + "/" + QuestionStatuses.Count;

		public ICommand SpeakCommand => new Command(() =>
		{
			if (FrontIsVisible)
				_textToSpeech.Speak(FrontText, new CultureInfo(_frontLanguage.Tag()));
			else
				_textToSpeech.Speak(BackText, new CultureInfo(_backLanguage.Tag()));
		});

		public ICommand RotateCommand => new Command(() => { BackIsVisible = !BackIsVisible; });

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_examiner = (IExaminer) parameters["examiner"];
			_correctAnswersProgressCalculator = _correctAnswerRatioTrackerFactory(_examiner);

			_examiner.SessionEnded += HandleSessionEnd;

			ResetQuestionStatuses(_examiner.QuestionsCount);
			TryShowNextQuestion();
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private async void HandleSessionEnd(object sender, QuestionResultsEventArgs args)
		{
			_canAnswer = false;
			var totalCorrectAnswersRatio = _correctAnswersProgressCalculator.CalculateProgress(args);
			var summary = $"{AppResources.Known}: {args.Results.Count(x => x.IsKnown)} \n" +
			              $"{AppResources.Unknown}: {args.Results.Count(x => !x.IsKnown)}\n" +
			              totalCorrectAnswersRatio + "%";
			await _dialogService.DisplayAlertAsync(
				AppResources.EndOfSession,
				summary,
				"OK");
			if (!args.Results.All(r => r.IsKnown))
			{
				ResetQuestionStatuses(args.NumberOfQuestionsInNextSession);
			}
			else
			{
				_examiner.SessionEnded -= HandleSessionEnd;
				await _navigationService.GoBackAsync();
			}

			_canAnswer = true;
		}

		private void ResetQuestionStatuses(int questionsCount)
		{
			CurrentQuestionNumber = 0;

			QuestionStatuses.Clear();
			for (var i = 0; i < questionsCount; ++i)
				QuestionStatuses.Add(new MulticolorbarItem
				{
					Color = Color.Gray,
					Value = 1
				});
		}

		private void TryShowNextQuestion()
		{
			if (_examiner.TryAskNextQuestion(out var question))
			{
				Revealed = false;
				FrontText = question.Front;
				BackText = question.Back;
				_frontLanguage = question.FrontLanguage;
				_backLanguage = question.BackLanguage;
				ImageUri = question.ImageUrl != null
					? new Uri(question.ImageUrl)
					: null;
			}
		}
	}
}