using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
	public class ExaminerBuilder
	{
		private static readonly Random Rnd = new Random();
		private AskingMode _askingMode;
		private IEnumerable<Question> _questions;
		private bool _shuffle;
		private int _maximum = Int32.MaxValue;

		public ExaminerBuilder WithLessons(IEnumerable<Lesson> lessons)
		{
			_questions = lessons.SelectMany(l =>
				l.Flashcards.Select(f => new Question(f, l.FrontLanguage, l.BackLanguage)));
			return this;
		}

		public ExaminerBuilder WithAskingMode(AskingMode askingMode)
		{
			_askingMode = askingMode;
			return this;
		}

		public ExaminerBuilder WithShuffling(bool shuffle)
		{
			_shuffle = shuffle;
			return this;
		}

		public ExaminerBuilder WithMaximumFlashcards(int maximum)
		{
			_maximum = maximum;
			return this;
		}

		public IExaminer Build()
		{
			if (_questions == null)
				throw new InvalidOperationException();

			return (_shuffle ? _questions.Shuffle() : _questions)
				.Take(_maximum)
				.Pipe(
					questions => 
					_askingMode == AskingMode.Front ? new Examiner(questions) :
					_askingMode == AskingMode.Back ? new Examiner(questions.Select(Invert)) :
					_askingMode == AskingMode.Random ? new Examiner(questions.Select(Random)) :
					throw new ArgumentOutOfRangeException());

			Question Invert(Question question)
			{
				var invertedFlashcard = new Flashcard
				{
					Front = question.Back,
					Back = question.Front,
					ImageUrl = question.ImageUrl,
					Id = question.InternalFlashcard.Id,
					LessonId = question.InternalFlashcard.LessonId
				};
				return new Question(
					frontLanguage: question.BackLanguage, 
					backLanguage: question.FrontLanguage,
					flashcard: invertedFlashcard);
			}

			Question Random(Question question)
			{
				return Rnd.NextDouble() > 0.5 ? question : Invert(question);
			}
		}
	}
}