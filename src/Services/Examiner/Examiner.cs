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
	    private Flashcard _currentQuestion;

        public Examiner(IEnumerable<Flashcard> questions)
        {
            _questionsToAsk = new Queue<Flashcard>(questions);
        }

	    /// <exception cref="InvalidOperationException">Question not asked.</exception>
	    public void Answer(bool known)
        {
	        if (_currentQuestion is null)
		        throw new InvalidOperationException("Question not asked.");

			_answeredQuestions.Add(new AnsweredQuestion(_currentQuestion, known));

	        _currentQuestion = null;

	        if (!_questionsToAsk.Any())
	        {
		        var answeredQuestions = new List<AnsweredQuestion>(_answeredQuestions);

		        ReloadQuestions();

				var eventArgs = new QuestionResultsEventArgs(
			        answeredQuestions,
			        _questionsToAsk.Count);

				SessionEnded?.Invoke(this, eventArgs);
			}

			void ReloadQuestions()
	        {
		        foreach (var answeredQuestion in _answeredQuestions.Where(q => !q.IsKnown))
		        {
			        _questionsToAsk.Enqueue(answeredQuestion.Flashcard);
		        }
		        _answeredQuestions.Clear();
	        }
        }

	    /// <exception cref="InvalidOperationException">Previous question has not been answered.</exception>
		public bool TryAskNextQuestion(out Flashcard flashcard)
        {
            if (_currentQuestion != null)
                throw new InvalidOperationException("Previous question has not been answered.");

            if (_questionsToAsk.Any())
            {
                flashcard = _questionsToAsk.Dequeue();
	            _currentQuestion = flashcard;
                return true;
            }

            flashcard = null;
            return false;
        }

	    public event EventHandler<QuestionResultsEventArgs> SessionEnded;
	    public int QuestionsCount => _questionsToAsk.Count;
    }
}