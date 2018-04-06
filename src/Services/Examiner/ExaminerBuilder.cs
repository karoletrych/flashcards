using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
    public class ExaminerBuilder
    {
        private AskingMode _askingMode;
        private IEnumerable<Flashcard> _flashcards;
	    private bool _shuffle = false;

        public ExaminerBuilder WithFlashcards(IEnumerable<Flashcard> flashcards)
        {
            _flashcards = flashcards;
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
		private static readonly Random Rnd = new Random();
		public IExaminer Build()
        {

			if (_flashcards == null)
                throw new InvalidOperationException();
	        var flashcardList = _flashcards.ToList();

	        if (_shuffle)
		        flashcardList.Shuffle();

            switch (_askingMode)
            {
                case AskingMode.Front:
                    return new Examiner(flashcardList);
                case AskingMode.Back:
                    return new Examiner(flashcardList.Select(Invert));
                case AskingMode.Random:
                    return new Examiner(flashcardList.Select(Random));
                default:
                    throw new ArgumentOutOfRangeException();
            }

	        Flashcard Invert(Flashcard flashcard)
	        {
		        return new Flashcard
		        {
			        Back = flashcard.Front,
			        Front = flashcard.Back,
			        Id = flashcard.Id,
			        ImageUrl = flashcard.ImageUrl,
			        LessonId = flashcard.LessonId
		        };
	        }

	        Flashcard Random(Flashcard flashcard)
	        {
		        return Rnd.NextDouble() > 0.5 ? flashcard : Invert(flashcard);
	        }

		}
	}
}