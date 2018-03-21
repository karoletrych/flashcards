using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
    public class Examiner : IExaminer
    {
        private readonly IList<Question> _askedQuestions = new List<Question>();
		private readonly Queue<Question> _questionsToAsk;

        public Examiner(IEnumerable<Question> questions)
        {
            _questionsToAsk = new Queue<Question>(questions);
        }

        public IEnumerable<Question> Questions =>
            _askedQuestions.Concat(_questionsToAsk);

        public TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults { get; } 
            = new TaskCompletionSource<IEnumerable<QuestionResult>>();

        public void Answer(bool known)
        {
	        var question = _askedQuestions.Last();
	        question.Status =
		        known ? QuestionStatus.Known : QuestionStatus.Unknown;
		}

        public bool TryAskNextQuestion(out Flashcard flashcard)
        {
            if (_askedQuestions.Any() && _askedQuestions.Last().Status == QuestionStatus.NotAnswered)
                throw new InvalidOperationException("Previous question has not been answered.");

            if (_questionsToAsk.Any())
            {
                var question = _questionsToAsk.Dequeue();
                _askedQuestions.Add(question);
	            flashcard = question.Flashcard;
                return true;
            }

			QuestionResults.SetResult(_askedQuestions.Select(q => new QuestionResult(q.Flashcard, IsKnown(q.Status))));
	        flashcard = null;
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