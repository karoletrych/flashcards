using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

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
        }

        public IEnumerable<FlashcardQuestion> Questions =>
            _askedQuestions.Concat(_questionsToAsk);

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

            question = null;
            return false;
        }
    }
}