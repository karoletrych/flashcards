using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FlashCards.Services;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly IPageDialogService _dialogService;
        private readonly IExaminerModelFactory _examinerModelFactory;
        private readonly INavigationService _navigationService;

        private string _backText;
        private ExaminerModel _examinerModel; // TODO: make it readonly (remove Prism)
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
            IExaminerModelFactory examinerModelFactory,
            INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _examinerModelFactory = examinerModelFactory;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        // just for View binding
        public AskingQuestionsViewModel()
        {
        }

        public bool ShowBackButtonIsVisible => !FrontIsVisible;

        public ICommand UserAnswerCommand => new Command<bool>(known =>
        {
            _examinerModel.Answer(isKnown: known);

            QuestionStatuses = _examinerModel.QuestionsStatuses.Select(x =>
            {
                switch (x)
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
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }
            }).ToList();

            FrontIsVisible = false;
            ShowNextQuestionOrEnd();
        });

        public ICommand ShowBackCommand => new Command(() =>
        {
            QuestionStatuses = _examinerModel.QuestionsStatuses.Select(questionStatus =>
            {
                switch (questionStatus)
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
                        throw new ArgumentOutOfRangeException(nameof(questionStatus), questionStatus, null);
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
                OnPropertyChanged(nameof(ShowBackButtonIsVisible));
            }
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            var lessonId = (int) parameters["lessonId"];
            _examinerModel = await _examinerModelFactory.Create(lessonId);
             
            ShowNextQuestionOrEnd();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ShowNextQuestionOrEnd()
        {
            if (_examinerModel.TryAskNextQuestion(out var question))
            {
                FrontText = question.FrontText;
                BackText = question.BackText;
            }
            else
            {
                await _dialogService.DisplayAlertAsync("Koniec",
                    $"Znane: {_examinerModel.QuestionsStatuses.Count(x => x == QuestionStatus.Known)} \n" +
                    $"Nieznane: {_examinerModel.QuestionsStatuses.Count(x => x == QuestionStatus.Unknown)}",
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