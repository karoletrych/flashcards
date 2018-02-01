using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.ViewModel;

namespace FlashCards.Model
{
    internal class TrainingSet
    {
        private readonly Queue<Question> _questionsToAsk;
        private readonly IList<Question> _answeredQuestions = new List<Question>();
        private Question _currentQuestion;
        
        public TrainingSet(IList<Question> questions)
        {
            _questionsToAsk = new Queue<Question>(questions);
        }

        public IEnumerable<QuestionStatus> QuestionsStatuses => 
            _answeredQuestions.Select(q=>q.Status)
                .Concat(_questionsToAsk.Select(q => q.Status));
    
        public void AnswerKnow()
        {
            _currentQuestion.Status = QuestionStatus.AnsweredCorrectly;
            _answeredQuestions.Add(_currentQuestion);
        }

        public void AnswerDontKnow()
        {
            _currentQuestion.Status = QuestionStatus.AnsweredWrongly;
            _answeredQuestions.Add(_currentQuestion);
        }

        public Question GetNextQuestion()
        {
            if(_currentQuestion != null && _currentQuestion.Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Question has not been answered.");
            var newQuestion = _questionsToAsk.Dequeue();
            _currentQuestion = newQuestion;
            return newQuestion;
        }
    }

    internal class Question
    {
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public QuestionStatus Status { get; set; }
    }
}