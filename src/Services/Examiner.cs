using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
    public class Examiner
    {
        private readonly bool _repeatFailedQuestions;
        private readonly IList<Question> _askedQuestions = new List<Question>();
        private readonly Queue<Question> _questionsToAsk;

        public Examiner(IEnumerable<Question> questions, bool repeatFailedQuestions)
        {
            _repeatFailedQuestions = repeatFailedQuestions;
            _questionsToAsk = new Queue<Question>(questions);
        }

        public IEnumerable<Question> Questions =>
            _askedQuestions.Concat(_questionsToAsk);

        public TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults { get; } 
            = new TaskCompletionSource<IEnumerable<QuestionResult>>();

        public void Answer(bool isKnown)
        {
            _askedQuestions.Last().Status =
                isKnown ? QuestionStatus.Known : QuestionStatus.Unknown;
        }

        public bool TryAskNextQuestion(out Question question)
        {
            if (_askedQuestions.Any() && _askedQuestions.Last().Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Previous question has not been answered.");

            if (_questionsToAsk.Any())
            {
                question = _questionsToAsk.Dequeue();
                _askedQuestions.Add(question);
                return true;
            }

            QuestionResults.SetResult(_askedQuestions.Select(q => new QuestionResult(q.Flashcard, IsKnown(q.Status))));
            question = null;
            return false;
        }

        private static bool IsKnown(QuestionStatus status)
        {
            switch (status)
            {

                case QuestionStatus.Known:
                    return true;
                case QuestionStatus.Unknown:
                    return false;
                case QuestionStatus.NotAnswered:
                    throw new ArgumentException("should be answered at this stage");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}