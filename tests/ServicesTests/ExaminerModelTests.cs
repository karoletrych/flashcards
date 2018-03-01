using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class ExaminerModelTests
    {
        public ExaminerModelTests()
        {
            _examiner = new Examiner(_questions);
        }

        private readonly Examiner _examiner;

        private readonly IList<Flashcard> _questions = new[]
        {
            new Flashcard{Front = "dog", Back = "pies"},
            new Flashcard{Front = "cat", Back = "kot"},
            new Flashcard{Front = "duck", Back = "kaczka"}
        };

        [Fact]
        public void WhenNoQuestions_FalseIsReturned()
        {

            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(true);
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(false);
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(false);

            Assert.False(_examiner.TryAskNextQuestion(out _));
        }

        [Fact]
        public void AsksCorrectQuestions()
        {
            _examiner.TryAskNextQuestion(out var q0);
            _examiner.Answer(true);
            _examiner.TryAskNextQuestion(out var q1);
            _examiner.Answer(false);
            _examiner.TryAskNextQuestion(out var q2);
            _examiner.Answer(false);

            Assert.Equal(q0.Flashcard, _questions[0]);
            Assert.Equal(q1.Flashcard, _questions[1]);
            Assert.Equal(q2.Flashcard, _questions[2]);
        }

        [Fact]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(true);
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(false);

            var statuses = _examiner.Questions.Select(q => q.Status).ToList();
            Assert.Equal(QuestionStatus.Known, statuses[0]);
            Assert.Equal(QuestionStatus.Unknown, statuses[1]);
            Assert.Equal(QuestionStatus.NotAnswered, statuses[2]);
        }

        [Fact]
        public void TrainingSetThrowsException_QuestionWasNotAnsweredAndNextIsAsked()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    _examiner.TryAskNextQuestion(out _);
                    _examiner.TryAskNextQuestion(out _);
                });
        }
    }
}