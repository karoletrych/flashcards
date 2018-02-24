using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FlashCards.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Flashcards.ViewModels
{
    public class AskingQuestionsViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly IExaminerModelFactory _examinerModelFactory;
        private bool _frontIsVisible;
        private Examiner _examiner; // TODO: make it readonly (remove Prism)
        private string _frontText;

        private IList<StepItem> _questionStatuses = new List<StepItem>
        {
            new StepItem {Color = Color.Gray, Value = 1}
        };

        private string _backText;

        public AskingQuestionsViewModel(IExaminerModelFactory examinerModelFactory)
        {
            _examinerModelFactory = examinerModelFactory;
        }

        public bool ShowBackButtonIsVisible => !FrontIsVisible;

        public ICommand UserAnswerCommand => new Command<bool>(known =>
        {
            _examiner.Answer(isKnown: known);

            QuestionStatuses = _examiner.QuestionsStatuses.Select(x =>
            {
                switch (x)
                {
                    case QuestionStatus.Known:
                        return new StepItem {Color = Color.GreenYellow, Value = 1};
                    case QuestionStatus.Unknown:
                        return new StepItem {Color = Color.Red, Value = 1};
                    case QuestionStatus.NotAnswered:
                        return new StepItem {Color = Color.Gray, Value = 1};
                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }
            }).ToList();

            FrontIsVisible = false;
            ShowNextQuestion();
        });

        public ICommand ShowBackCommand => new Command(() =>
        {
            QuestionStatuses = _examiner.QuestionsStatuses.Select(questionStatus =>
            {
                switch (questionStatus)
                {
                    case QuestionStatus.Known:
                        return new StepItem {Color = Color.GreenYellow, Value = 1};
                    case QuestionStatus.Unknown:
                        return new StepItem {Color = Color.Red, Value = 1};
                    case QuestionStatus.NotAnswered:
                        return new StepItem {Color = Color.Gray, Value = 1};
                    default:
                        throw new ArgumentOutOfRangeException(nameof(questionStatus), questionStatus, null);
                }
            }).ToList();

            FrontIsVisible = true;
        });


        public IList<StepItem> QuestionStatuses
        {
            get => _questionStatuses;
            set
            {
                _questionStatuses = value;
                OnPropertyChanged();
            }
        }

        public string FrontText
        {
            get => _frontText;
            set
            {
                _frontText = value;
                OnPropertyChanged();
            }
        }

        public string BackText
        {
            get => _backText;
            set
            {
                _backText = value;
                OnPropertyChanged();
            }
        }

        public bool FrontIsVisible
        {
            get => _frontIsVisible;
            set
            {
                if(_frontIsVisible == value)
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
            var lessonId = (int)parameters["lessonId"];
            _examiner = await _examinerModelFactory.Create(lessonId);
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            var question = _examiner.GetNextQuestion();
            FrontText = question.FrontText;
            BackText = question.BackText;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }
    }
}