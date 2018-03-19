using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;

namespace Flashcards.Services
{
    public class ExaminerBuilder
    {
        private AskingMode _askingMode;
        private IEnumerable<Flashcard> _flashcards;
        private bool _repeatQuestions;

        public ExaminerBuilder WithFlashcards(IEnumerable<Flashcard> flashcards)
        {
            _flashcards = flashcards;
            return this;
        }

        public ExaminerBuilder WithRepeatingQuestions(bool repeating)
        {
            _repeatQuestions = true;
            return this;
        }

        public ExaminerBuilder WithAskingMode(AskingMode askingMode)
        {
            _askingMode = askingMode;
            return this;
        }

        public Examiner Build()
        {
            if (_flashcards == null)
                throw new InvalidOperationException();

            var random = new Random();

            switch (_askingMode)
            {
                case AskingMode.Front:
                    return new Examiner(_flashcards.Select(flashcard => new Question(flashcard)), _repeatQuestions);
                case AskingMode.Back:
                    return new Examiner(_flashcards.Select(flashcard => new Question(Invert(flashcard))),
                        _repeatQuestions);
                case AskingMode.Random:
                    return new Examiner(
                        _flashcards.Select(flashcard =>
                            new Question(random.NextDouble() > 0.5 ? flashcard : Invert(flashcard))), _repeatQuestions);
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
        }
    }
}