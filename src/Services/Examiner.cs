using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
    public class FlashcardQuestion
    {
        public FlashcardQuestion(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }

        public Flashcard Flashcard { get; }
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

        public Examiner(IEnumerable<Flashcard> questions)
        {
            _questionsToAsk = new Queue<FlashcardQuestion>(
                questions.Select(f => new FlashcardQuestion(f)));
			QuestionResults = new TaskCompletionSource<IEnumerable<QuestionResult>>();
        }

        public IEnumerable<FlashcardQuestion> Questions =>
            _askedQuestions.Concat(_questionsToAsk);

	    public TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults { get; }

        public void Answer(bool isKnown)
        {
            _askedQuestions.Last().Status =
                isKnown ? QuestionStatus.Known : QuestionStatus.Unknown;
        }

        public bool TryAskNextQuestion(out FlashcardQuestion question)
        {
            if (_askedQuestions.Any() && _askedQuestions.Last().Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Previous question has not been answered.");

            if (_questionsToAsk.Any())
            {
                var newQuestion = _questionsToAsk.Dequeue();
                _askedQuestions.Add(newQuestion);
                question = newQuestion;
                return true;
            }

            QuestionResults.SetResult(_askedQuestions.Select(q => new QuestionResult(q.Flashcard, IsKnown(q.Status))));
            question = null;
            return false;
        }

        private bool IsKnown(QuestionStatus status)
        {
            switch (status)
            {
                case QuestionStatus.NotAnswered:
                    throw new ArgumentException("should be answered at this stage");
                case QuestionStatus.Known:
                    return true;
                case QuestionStatus.Unknown:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
