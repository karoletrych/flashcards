using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
	public class RepeatingExaminer : IRepeatingExaminer
	{
		public abstract class NextQuestion
		{
		}

		public class SomeQuestion : NextQuestion
		{
			public SomeQuestion(Flashcard question)
			{
				Question = question;
			}

			public Flashcard Question { get; }
		}

		public class NoQuestions : NextQuestion
		{
		}

		public class NextSessionQuestion : NextQuestion
		{
			public NextSessionQuestion(Flashcard question)
			{
				Question = question;
			}

			public Flashcard Question { get; }
		}

		private Examiner _examiner;

		public RepeatingExaminer(IEnumerable<Question> questions)
		{
			_examiner = new Examiner(questions);
		}

		public IEnumerable<Question> Questions => _examiner.Questions;

		public TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults => 
			_examiner.QuestionResults;

		public void Answer(bool known)
		{
			_examiner.Answer(known);
		}

		public NextQuestion TryAskNextQuestion()
		{
			if (_examiner.TryAskNextQuestion(out var question))
				return new SomeQuestion(question);

			var unknownQuestions = 
				_examiner
					.Questions
					.Where(q => q.Status == QuestionStatus.Unknown)
					.ToList();
			if (unknownQuestions.Any())
			{
				unknownQuestions.ForEach(q => q.Status = QuestionStatus.NotAnswered);

				_examiner = new Examiner(unknownQuestions);

				_examiner.TryAskNextQuestion(out question);
				return new NextSessionQuestion(question);
			}
			return new NoQuestions();
		}
	}
}