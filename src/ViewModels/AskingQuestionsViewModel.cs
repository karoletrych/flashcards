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

        private string _backText;
        private Examiner _examiner;
        private bool _frontIsVisible;
        private string _frontText;

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

        public ICommand UserAnswerCommand => new Command<bool>(known =>
        {
            _examiner.Answer(isKnown: known);

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

            FrontIsVisible = false;
            ShowNextQuestionOrEnd();
        });

        public ICommand ShowBackCommand => new Command(() =>
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

            FrontIsVisible = true;
        });


        public IList<StepItem> QuestionStatuses
        {
            get => _questionStatuses;
            private set
            {
                _questionStatuses = value;
                OnPropertyChanged();
            }
        }

        public string FrontText
        {
            get => _frontText;
            private set
            {
                _frontText = value;
                OnPropertyChanged();
            }
        }

        public string BackText
        {
            get => _backText;
            private set
            {
                _backText = value;
                OnPropertyChanged();
            }
        }

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

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            _examiner = (Examiner) parameters["examiner"];
            ShowNextQuestionOrEnd();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ShowNextQuestionOrEnd()
        {
            if (_examiner.TryAskNextQuestion(out var question))
            {
                FrontText = question.Flashcard.Front;
                BackText = question.Flashcard.Back;
            }
            else
            {
                await _dialogService.DisplayAlertAsync("Koniec",
                    $"Znane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Known)} \n" +
                    $"Nieznane: {_examiner.Questions.Count(x => x.Status == QuestionStatus.Unknown)}",
                    "OK");
	            await _navigationService.GoBackAsync();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }
    }
}