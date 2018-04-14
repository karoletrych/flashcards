using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Localization;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Services.Examiner;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigatedAware
	{
		public AskingQuestionsViewModel(
			INavigationService navigationService,
			IPageDialogService dialogService,
			ITextToSpeech textToSpeech,
			Func<IExaminer, CorrectAnswersRatioTracker> correctAnswerRatioTrackerFactory)
		{
			_navigationService = navigationService;
			_dialogService = dialogService;
			_textToSpeech = textToSpeech;
			_correctAnswerRatioTrackerFactory = correctAnswerRatioTrackerFactory;
		}

		public int CurrentQuestionNumber { get; private set; } = 0;
		private CorrectAnswersRatioTracker _correctAnswersRatioTracker;

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_examiner = (IExaminer)parameters["examiner"];
			_correctAnswersRatioTracker = _correctAnswerRatioTrackerFactory(_examiner);

			_examiner.SessionEnded +=
				async (sender, args) =>
				{
					_canAnswer = false;
					await _dialogService.DisplayAlertAsync(AppResources.EndOfSession,
						$"{AppResources.Known}: {args.Results.Count(x => x.IsKnown)} \n" +
						$"{AppResources.Unknown}: {args.Results.Count(x => !x.IsKnown)}\n" +
						_correctAnswersRatioTracker.Progress + "%",
						"OK");
					if (!args.Results.All(r => r.IsKnown))
						ResetQuestionStatuses(args.NumberOfQuestionsInNextSession);
					else
						await _navigationService.GoBackAsync();
					_canAnswer = true;
				};

			ResetQuestionStatuses(_examiner.QuestionsCount);
			TryShowNextQuestion();
		}

		private void ResetQuestionStatuses(int questionsCount)
		{
			CurrentQuestionNumber = 0;

			QuestionStatuses.Clear();
			for (var i = 0; i < questionsCount; ++i)
			{
				QuestionStatuses.Add(new MulticolorbarItem {Color = Color.Gray, Value = 1});
			}

			OnPropertyChanged(nameof(QuestionsProgress));
		}

		private readonly IPageDialogService _dialogService;
		private readonly ITextToSpeech _textToSpeech;
		private readonly Func<IExaminer, CorrectAnswersRatioTracker> _correctAnswerRatioTrackerFactory;

		private readonly INavigationService _navigationService;

		private IExaminer _examiner;

		private bool _backIsVisible;
		private Language _frontLanguage;
		private Language _backLanguage;
		private bool _canAnswer = true;

		public AskingQuestionsViewModel()
		{
		}

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
				OnPropertyChanged();
				OnPropertyChanged(nameof(FrontIsVisible));
			}
		}
		
		public ICommand UserAnswerCommand => new Command<bool>(known =>
		{
			QuestionStatuses[CurrentQuestionNumber] =
				known
					? new MulticolorbarItem { Color = Color.LawnGreen, Value = 1 }
					: new MulticolorbarItem { Color = Color.Red, Value = 1 };

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
			if(FrontIsVisible)
				_textToSpeech.Speak(FrontText, new CultureInfo(_frontLanguage.Tag()));
			else
				_textToSpeech.Speak(BackText, new CultureInfo(_backLanguage.Tag()));
		});

		public ICommand RotateCommand => new Command(() => { BackIsVisible = !BackIsVisible; });

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

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

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
		}
	}
}