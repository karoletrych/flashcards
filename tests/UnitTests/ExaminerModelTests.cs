using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.Services;
using Xunit;

namespace Flashcards.UnitTests
{
    public class ExaminerModelTests
    {
        public ExaminerModelTests()
        {
            _lesson = new Examiner(_questions);
        }

        private readonly Examiner _lesson;

        private readonly IList<FlashcardQuestion> _questions = new[]
        {
            new FlashcardQuestion("dog", "pies"),
            new FlashcardQuestion("cat", "kot"),
            new FlashcardQuestion("duck", "kaczka")
        };

        [Fact]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _lesson.GetNextQuestion();
            _lesson.Answer(true);
            _lesson.GetNextQuestion();
            _lesson.Answer(false);

            var statuses = _lesson.QuestionsStatuses.ToList();
            Assert.Equal(QuestionStatus.Known, statuses[0]);
            Assert.Equal(QuestionStatus.Unknown, statuses[1]);
            Assert.Equal(QuestionStatus.NotAnswered, statuses[2]);
        }

        [Fact]
        public void TrainingSetReturnsQuestions()
        {
            var q0 = _lesson.GetNextQuestion();
            _lesson.Answer(true);
            var q1 = _lesson.GetNextQuestion();
            _lesson.Answer(false);
            var q2 = _lesson.GetNextQuestion();
            _lesson.Answer(false);

            Assert.Equal(q0, _questions[0]);
            Assert.Equal(q1, _questions[1]);
            Assert.Equal(q2, _questions[2]);
        }

        [Fact]
        public void TrainingSetThrowsException_WhenAnswerWasNotSuppliedAndNextQuestionIsRetrieved()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    _lesson.GetNextQuestion();
                    _lesson.GetNextQuestion();
                });
        }
    }
}