using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Services;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigationAware
	{
		private readonly IPageDialogService _dialogService;
		private readonly INavigationService _navigationService;

		private IRepeatingExaminer _examiner;
		private bool _frontIsVisible;

		private IList<StepItem> _questionStatuses = new List<StepItem>
		{
			new StepItem
			{
				Color = Color.Gray,
				Value = 1
			}
		};

		public AskingQuestionsViewModel(
			INavigationService navigationService,
			IPageDialogService dialogService)
		{
			_navigationService = navigationService;
			_dialogService = dialogService;
		}

		// just for View binding
		public AskingQuestionsViewModel()
		{
		}

		public bool BackIsVisible => !FrontIsVisible;

		public string FrontText {get; private set;}
		public string BackText { get; private set; }

		public IList<StepItem> QuestionStatuses
		{
			get => _questionStatuses;
			private set
			{
				_questionStatuses = value;
				OnPropertyChanged();
			}
		}


		public Uri ImageUri { get; set; }

		public bool FrontIsVisible
		{
			get => _frontIsVisible;
			private set
			{
				if (_frontIsVisible == value)
					return;
				_frontIsVisible = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(BackIsVisible));
			}
		}

		public ICommand UserAnswerCommand => new Command<bool>(known =>
		{
			_examiner.Answer(known: known);


			FrontIsVisible = false;
			ShowNextQuestionOrFinishAsking();

			UpdateQuestionStatuses();
		});

		public ICommand ShowBackCommand => new Command(() =>
		{
			FrontIsVisible = true;

			UpdateQuestionStatuses();
		});


		public void OnNavigatedTo(NavigationParameters parameters)
		{
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_examiner = (IRepeatingExaminer) parameters["examiner"];
			ShowNextQuestionOrFinishAsking();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void UpdateQuestionStatuses()
		{
			QuestionStatuses = _examiner.Questions.Select(question =>
			{
				switch (question.Status)
				{
					case QuestionStatus.Known:
						return new StepItem
						{
							Color = Color.GreenYellow,
							Value = 1
						};
					case QuestionStatus.Unknown:
						return new StepItem
						{
							Color = Color.Red,
							Value = 1
						};
					case QuestionStatus.NotAnswered:
						return new StepItem
						{
							Color = Color.Gray,
							Value = 1
						};
					default:
						throw new ArgumentOutOfRangeException(nameof(question), question, null);
				}
			}).ToList();
		}

		private async void ShowNextQuestionOrFinishAsking()
		{
			var question = _examiner.TryAskNextQuestion();
			switch (question)
			{
				case RepeatingExaminer.NextSessionQuestion nextSessionQuestion:
					await _dialogService.DisplayAlertAsync("Koniec sesji z pytaniami",
						$"Znane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Known)} \n" +
						$"Nieznane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Unknown)}",
						"OK");
					FrontText = nextSessionQuestion.Question.Front;
					BackText = nextSessionQuestion.Question.Back;
					ImageUri = nextSessionQuestion.Question.ImageUrl != null
						? new Uri(nextSessionQuestion.Question.ImageUrl)
						: null;
					break;
				case RepeatingExaminer.NoQuestions noQuestions:
					await _dialogService.DisplayAlertAsync("Koniec",
						$"Znane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Known)} \n" +
						$"Nieznane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Unknown)}",
						"OK");
					await _navigationService.GoBackAsync();
					break;
				case RepeatingExaminer.SomeQuestion someQuestion:
					FrontText = someQuestion.Question.Front;
					BackText = someQuestion.Question.Back;
					ImageUri = someQuestion.Question.ImageUrl != null
						? new Uri(someQuestion.Question.ImageUrl)
						: null;
					break;
			}
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
		}
	}
}