using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Xunit;

namespace Flashcards.UnitTests
{
    public class QuestionsAskerTests
    {
        public QuestionsAskerTests()
        {
            _lesson = new QuestionAsker(_questions);
        }

        private readonly QuestionAsker _lesson;

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
            Assert.Equal(QuestionStatus.AnsweredCorrectly, statuses[0]);
            Assert.Equal(QuestionStatus.AnsweredBadly, statuses[1]);
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