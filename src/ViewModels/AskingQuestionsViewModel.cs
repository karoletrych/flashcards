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
        private readonly ExaminerFactory _examinerFactory;
        private bool _answerIsVisible;
        private Examiner _examiner; // TODO: make it readonly (remove Prism)
        private string _questionAnswerText;

        private IList<StepItem> _questionStatuses = new List<StepItem>
        {
            new StepItem {Color = Color.Gray, Value = 1}
        };

        public AskingQuestionsViewModel(ExaminerFactory examinerFactory)
        {
            _examinerFactory = examinerFactory;
        }

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

            QuestionAnswerText = _examiner.GetNextQuestion().FrontText;
            QuestionIsVisible = true;
        });

        public ICommand ShowAnswerCommand => new Command(() =>
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

            AnswerIsVisible = true;
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

        public string QuestionAnswerText
        {
            get => _questionAnswerText;
            set
            {
                _questionAnswerText = value;
                OnPropertyChanged();
            }
        }


        public bool AnswerIsVisible
        {
            get => _answerIsVisible;
            set
            {
                if(_answerIsVisible == value)
                    return;
                _answerIsVisible = value;
                QuestionIsVisible = !value;
                OnPropertyChanged();
            }
        }

        public bool QuestionIsVisible
        {
            get => !_answerIsVisible;
            set
            {
                if (_answerIsVisible == !value)
                    return;
                AnswerIsVisible = !value;
                OnPropertyChanged();
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
            var lessonId = (int)parameters["lessonId"];
            _examiner = _examinerFactory.Create(lessonId).Result;
            QuestionAnswerText = _examiner.GetNextQuestion().FrontText;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }
    }
}