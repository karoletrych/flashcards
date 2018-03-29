using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Services;
using Flashcards.Services.Examiner;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
	public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigatedAware
	{
		private readonly IPageDialogService _dialogService;
		private readonly INavigationService _navigationService;

		private IExaminer _examiner;
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
			ShowNextQuestion();

			UpdateQuestionStatuses();
		});

		public ICommand ShowBackCommand => new Command(() =>
		{
			FrontIsVisible = true;

			UpdateQuestionStatuses();
		});


		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_examiner = (IExaminer)parameters["examiner"];
			_examiner.SessionEnded +=
				async (sender, args) =>
				{
					await _dialogService.DisplayAlertAsync("Koniec",
						$"Znane: {args.Results.Count(x => x.IsKnown)} \n" +
						$"Nieznane: {args.Results.Count(x => !x.IsKnown)}",
						"OK");
					if (args.Results.All(r => r.IsKnown))
					{
						await _navigationService.GoBackAsync();
					}
				};
			ShowNextQuestion();
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void UpdateQuestionStatuses()
		{
		}

		private void ShowNextQuestion()
		{
			if (_examiner.TryAskNextQuestion(out var question))
			{
				FrontText = question.Front;
				BackText = question.Back;
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