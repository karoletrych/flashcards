using System;
using System.Collections.Generic;
using System.Linq;

namespace Flashcards.Models
{
    public class Question
    {
        public Question(string questionText, string answerText)
        {
            QuestionText = questionText;
            AnswerText = answerText;
        }

        public string QuestionText { get; }
        public string AnswerText { get; }
        public QuestionStatus Status { get; set; }
    }

    public enum QuestionStatus
    {
        NotAnswered,
        AnsweredCorrectly,
        AnsweredBadly
    }

    public class QuestionAsker
    {
        private readonly IList<Question> _askedQuestions = new List<Question>();
        private readonly Queue<Question> _questionsToAsk;

        public QuestionAsker(IList<Question> questions)
        {
            _questionsToAsk = new Queue<Question>(questions);
        }

        public IEnumerable<QuestionStatus> QuestionsStatuses =>
            _askedQuestions.Select(q => q.Status)
                .Concat(_questionsToAsk.Select(q => q.Status));

        public string CurrentQuestionAnswer => _askedQuestions.Last().AnswerText;

        public void Answer(bool isKnown)
        {
            _askedQuestions.Last().Status =
                isKnown ? QuestionStatus.AnsweredCorrectly : QuestionStatus.AnsweredBadly;
        }

        public Question GetNextQuestion()
        {
            if (_askedQuestions.Any() && _askedQuestions.Last().Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Previous question has not been answered.");
            var newQuestion = _questionsToAsk.Dequeue();
            _askedQuestions.Add(newQuestion);
            return newQuestion;
        }
    }
}