using FlashCards.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashCards.UnitTests
{
    [TestFixture]
    public class QuestionsSetTests
    {
        [SetUp]
        public void SetUp()
        {
            _questions = new[]
            {
                new Question {QuestionText = "dog", AnswerText = "pies"},
                new Question {QuestionText = "cat", AnswerText = "kot"},
                new Question {QuestionText = "duck", AnswerText = "kaczka"}
            };
            _lesson = new LessonModel(_questions);
        }

        private LessonModel _lesson;
        private IList<Question> _questions;

        [Test]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _lesson.GetNextQuestion();
            _lesson.Answer(isKnown: true);
            _lesson.GetNextQuestion();
            _lesson.Answer(isKnown: false);

            var statuses = _lesson.QuestionsStatuses.ToList();
            Assert.That(statuses[0] == QuestionStatus.AnsweredCorrectly);
            Assert.That(statuses[1] == QuestionStatus.AnsweredBadly);
            Assert.That(statuses[2] == QuestionStatus.NotAnswered);
        }

        [Test]
        public void TrainingSetReturnsQuestions()
        {
            var q0 = _lesson.GetNextQuestion();
            _lesson.Answer(isKnown: true);
            var q1 = _lesson.GetNextQuestion();
            _lesson.Answer(isKnown: false);
            var q2 = _lesson.GetNextQuestion();
            _lesson.Answer(isKnown: false);

            Assert.That(_questions[0] == q0);
            Assert.That(_questions[1] == q1);
            Assert.That(_questions[2] == q2);
        }

        [Test]
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