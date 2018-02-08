using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.Model;
using Xunit;

namespace FlashCards.UnitTests
{
    public class QuestionsSetTests
    {
        public QuestionsSetTests()
        {
            _lesson = new LessonModel(_questions);
        }

        private readonly LessonModel _lesson;
        private readonly IList<Question> _questions = new[]
        {
            new Question("dog", "pies"),
            new Question("cat", "kot"),
            new Question("duck", "kaczka")
        };

        [Fact]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _lesson.GetNextQuestion();
            _lesson.Answer(true);
            _lesson.GetNextQuestion();
            _lesson.Answer(false);

            var statuses = _lesson.QuestionsStatuses.ToList();
            Assert.Equal(expected: QuestionStatus.AnsweredCorrectly, actual: statuses[0]);
            Assert.Equal(expected: QuestionStatus.AnsweredBadly, actual: statuses[1]);
            Assert.Equal(expected: QuestionStatus.NotAnswered, actual: statuses[2]);
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

            Assert.Equal(expected: q0, actual: _questions[0]);
            Assert.Equal(expected: q1, actual: _questions[1]);
            Assert.Equal(expected: q2, actual: _questions[2]);
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