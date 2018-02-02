using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashCards.Model
{
    internal class QuestionsSetModel
    {
        private readonly IList<Question> _answeredQuestions = new List<Question>();
        private readonly Queue<Question> _questionsToAsk;
        private Question _currentQuestion;

        public QuestionsSetModel(IList<Question> questions)
        {
            _questionsToAsk = new Queue<Question>(new List<Question>
            {
                new Question {QuestionText = "dog", AnswerText = "pies"},
                new Question {QuestionText = "cat", AnswerText = "kot"},
                new Question {QuestionText = "duck", AnswerText = "kaczka"}
            });
//            _questionsToAsk = new Queue<Question>(questions);
        }

        public IEnumerable<QuestionStatus> QuestionsStatuses =>
            _answeredQuestions.Select(q => q.Status)
                .Concat(_questionsToAsk.Select(q => q.Status));

        public string CurrentQuestionAnswer => _currentQuestion.AnswerText;

        public void AnswerKnow()
        {
            _currentQuestion.Status = QuestionStatus.AnsweredCorrectly;
            _answeredQuestions.Add(_currentQuestion);
        }

        public void AnswerDontKnow()
        {
            _currentQuestion.Status = QuestionStatus.AnsweredBadly;
            _answeredQuestions.Add(_currentQuestion);
        }
         
        public Question GetNextQuestion()
        {
            if (_currentQuestion != null && _currentQuestion.Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Question has not been answered.");
            var newQuestion = _questionsToAsk.Dequeue();
            _currentQuestion = newQuestion;
            return newQuestion;
        }
    }

    public enum QuestionStatus
    {
        NotAnswered,
        AnsweredCorrectly,
        AnsweredBadly
    }

    internal class Question
    {
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public QuestionStatus Status { get; set; }
    }
}