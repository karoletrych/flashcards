using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FlashCards.Model;
using Xamarin.Forms;

namespace FlashCards.ViewModel
{
    internal class QuestionsViewModel : INotifyPropertyChanged
    {
        private readonly QuestionsSetModel _questionsSetModel;
        private string _questionAnswerText;
        private bool _answerIsVisible;
        private IList<StepItem> _questionStatuses = new List<StepItem>{
            new StepItem {Color = Color.Gray, Value = 1}
        };

    public QuestionsViewModel()
        {
            _questionsSetModel = new QuestionsSetModel(new List<Question>());
            UserAnswerCommand = new Command<bool>(answer =>
            {
                if (answer)
                    _questionsSetModel.AnswerKnow();
                else
                    _questionsSetModel.AnswerDontKnow();

                QuestionStatuses = _questionsSetModel.QuestionsStatuses.Select(x =>
                {
                    switch (x)
                    {
                        case QuestionStatus.AnsweredCorrectly:
                            return new StepItem { Color = Color.GreenYellow, Value = 1 };
                        case QuestionStatus.AnsweredBadly:
                            return new StepItem { Color = Color.Red, Value = 1 };
                        case QuestionStatus.NotAnswered:
                            return new StepItem { Color = Color.Gray, Value = 1 };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(x), x, null);
                    }
                }).ToList();

                QuestionAnswerText = _questionsSetModel.GetNextQuestion().QuestionText;
                ShowQuestion();
            });

            ShowAnswerCommand = new Command(() =>
            {
                QuestionAnswerText = _questionsSetModel.CurrentQuestionAnswer;
                QuestionStatuses = _questionsSetModel.QuestionsStatuses.Select(questionStatus =>
                {
                    switch (questionStatus)
                    {
                        case QuestionStatus.AnsweredCorrectly:
                            return new StepItem { Color = Color.GreenYellow, Value = 1 };
                        case QuestionStatus.AnsweredBadly:
                            return new StepItem { Color = Color.Red, Value = 1 };
                        case QuestionStatus.NotAnswered:
                            return new StepItem { Color = Color.Gray, Value = 1 };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(questionStatus), questionStatus, null);
                    }
                }).ToList();

                ShowAnswer();
            });

            QuestionAnswerText = _questionsSetModel.GetNextQuestion().QuestionText;
        }

        public ICommand UserAnswerCommand { get; }
        public ICommand ShowAnswerCommand { get; }


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
                _answerIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool QuestionIsVisible
        {
            get => !_answerIsVisible;
            set
            {
                _answerIsVisible = !value;
                OnPropertyChanged();
            }
        }

        private void ShowAnswer()
        {
            QuestionIsVisible = false;
            AnswerIsVisible = true;
        }

        private void ShowQuestion()
        {
            QuestionIsVisible = true;
            AnswerIsVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }
    }
}