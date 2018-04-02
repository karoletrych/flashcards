using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Flashcards.Localization;
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
			IPageDialogService dialogService)
		{
			_navigationService = navigationService;
			_dialogService = dialogService;
		}

		private int _currentQuestionIndex = 0;

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_examiner = (IExaminer)parameters["examiner"];
			_examiner.SessionEnded += DisplayEndOfSessionAlert;

			ResetQuestionStatuses();

			ShowNextQuestion();
		}

		private void ResetQuestionStatuses()
		{
			for (var i = 0; i < _examiner.NumberOfQuestion; ++i)
			{
				QuestionStatuses.Add(new ColorbarItem {Color = Color.Gray, Value = 1});
			}

			OnPropertyChanged(nameof(QuestionStatuses));
		}

		private async void DisplayEndOfSessionAlert(object sender, QuestionResultsEventArgs args)
		{
			await _dialogService.DisplayAlertAsync(AppResources.EndOfSession,
				$"{AppResources.Known}: {args.Results.Count(x => x.IsKnown)} \n" +
				$"{AppResources.Unknown}: {args.Results.Count(x => !x.IsKnown)}",
				"OK");
			if (args.Results.All(r => r.IsKnown))
			{
				await _navigationService.GoBackAsync();
			}

			ResetQuestionStatuses();
		}

		private readonly IPageDialogService _dialogService;

		private readonly INavigationService _navigationService;

		private IExaminer _examiner;

		private bool _backIsVisible;

		public AskingQuestionsViewModel()
		{
		}

		public bool FrontIsVisible => !BackIsVisible;

		public string FrontText { get; private set; }

		public string BackText { get; private set; }

		public ObservableCollection<ColorbarItem> QuestionStatuses { get; } = 
			new ObservableCollection<ColorbarItem>();

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
			_examiner.Answer(known: known);

			QuestionStatuses[_currentQuestionIndex] =
				known
					? new ColorbarItem {Color = Color.LawnGreen, Value = 1}
					: new ColorbarItem {Color = Color.Red, Value = 1};
			OnPropertyChanged(nameof(QuestionStatuses));


			BackIsVisible = false;
			ShowNextQuestion();

			UpdateQuestionStatuses();
		});

		public ICommand ShowBackCommand => new Command(() =>
		{
			BackIsVisible = true;

			UpdateQuestionStatuses();
		});


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