using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashCards.Services
{
    public class FlashcardQuestion
    {
        public FlashcardQuestion(string frontText, string backText)
        {
            FrontText = frontText;
            BackText = backText;
            Status = QuestionStatus.NotAnswered;
        }

        public string FrontText { get; }
        public string BackText { get; }
        public QuestionStatus Status { get; set; }
    }

    public enum QuestionStatus
    {
        NotAnswered,
        Known,
        Unknown
    }

    public class Examiner
    {
        private readonly IList<FlashcardQuestion> _askedQuestions = new List<FlashcardQuestion>();
        private readonly Queue<FlashcardQuestion> _questionsToAsk;

        public Examiner(IEnumerable<FlashcardQuestion> questions)
        {
            _questionsToAsk = new Queue<FlashcardQuestion>(questions);
        }

        public IEnumerable<QuestionStatus> QuestionsStatuses =>
            _askedQuestions.Select(q => q.Status)
                .Concat(_questionsToAsk.Select(q => q.Status));

        public void Answer(bool isKnown)
        {
            _askedQuestions.Last().Status =
                isKnown ? QuestionStatus.Known : QuestionStatus.Unknown;
        }

        public FlashcardQuestion GetNextQuestion()
        {
            if (_askedQuestions.Any() && _askedQuestions.Last().Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Previous question has not been answered.");
            var newQuestion = _questionsToAsk.Dequeue();
            _askedQuestions.Add(newQuestion);
            return newQuestion;
        }
    }
}