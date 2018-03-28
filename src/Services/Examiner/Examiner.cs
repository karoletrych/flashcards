using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
    public class Examiner : IExaminer
    {
        private readonly IList<AnsweredQuestion> _answeredQuestions = new List<AnsweredQuestion>();
		private readonly Queue<Flashcard> _questionsToAsk;
	    private Flashcard _askedQuestion;

        public Examiner(IEnumerable<Flashcard> questions)
        {
            _questionsToAsk = new Queue<Flashcard>(questions);
        }

	    /// <exception cref="InvalidOperationException">Question not asked.</exception>
	    public void Answer(bool known)
        {
	        if (_askedQuestion is null)
		        throw new InvalidOperationException("Question not asked.");

			_answeredQuestions.Add(new AnsweredQuestion(_askedQuestion, known));
		}

	    /// <exception cref="InvalidOperationException">Previous question has not been answered.</exception>

	    public bool TryAskNextQuestion(out Flashcard flashcard)
        {
            if (_askedQuestion != null)
                throw new InvalidOperationException("Previous question has not been answered.");

            if (_questionsToAsk.Any())
            {
                flashcard = _questionsToAsk.Dequeue();
	            _askedQuestion = flashcard;
                return true;
            }

            RaiseQuestionsAnswered();
            flashcard = null;
            return false;
        }

        private void RaiseQuestionsAnswered()
        {
            var eventArgs = new QuestionResultsEventArgs(_answeredQuestions);
            QuestionsAnswered?.Invoke(this, eventArgs);
        }

        public event EventHandler<QuestionResultsEventArgs> QuestionsAnswered;
    }
}