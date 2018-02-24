using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.Services;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class ExaminerModelTests
    {
        public ExaminerModelTests()
        {
            _examiner = new ExaminerModel(_questions);
        }

        private readonly ExaminerModel _examiner;

        private readonly IList<FlashcardQuestion> _questions = new[]
        {
            new FlashcardQuestion("dog", "pies"),
            new FlashcardQuestion("cat", "kot"),
            new FlashcardQuestion("duck", "kaczka")
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

            Assert.Equal(q0, _questions[0]);
            Assert.Equal(q1, _questions[1]);
            Assert.Equal(q2, _questions[2]);
        }

        [Fact]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(true);
            _examiner.TryAskNextQuestion(out _);
            _examiner.Answer(false);

            var statuses = _examiner.QuestionsStatuses.ToList();
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