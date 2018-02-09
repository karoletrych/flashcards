using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Models;
using Xamarin.Forms;

namespace ViewModels
{
    public class AskingQuestionsViewModel : INotifyPropertyChanged
    {
        private readonly LessonModel _lessonModel;
        private string _questionAnswerText;
        private bool _answerIsVisible;
        private IList<StepItem> _questionStatuses = new List<StepItem>{
            new StepItem {Color = Color.Gray, Value = 1}
        };

    public AskingQuestionsViewModel()
        {
            _lessonModel = new LessonModel(new List<Question>());
            UserAnswerCommand = new Command<bool>(known =>
            {
                _lessonModel.Answer(isKnown: known);

                QuestionStatuses = _lessonModel.QuestionsStatuses.Select(x =>
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

                QuestionAnswerText = _lessonModel.GetNextQuestion().QuestionText;
                ShowQuestion();
            });

            ShowAnswerCommand = new Command(() =>
            {
                QuestionAnswerText = _lessonModel.CurrentQuestionAnswer;
                QuestionStatuses = _lessonModel.QuestionsStatuses.Select(questionStatus =>
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

            QuestionAnswerText = _lessonModel.GetNextQuestion().QuestionText;
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