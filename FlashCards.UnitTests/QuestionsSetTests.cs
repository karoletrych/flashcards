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
            _questionsSet = new QuestionsSetModel(_questions);
        }

        private QuestionsSetModel _questionsSet;
        private IList<Question> _questions;

        [Test]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _questionsSet.GetNextQuestion();
            _questionsSet.AnswerKnow();
            _questionsSet.GetNextQuestion();
            _questionsSet.AnswerDontKnow();

            var statuses = _questionsSet.QuestionsStatuses.ToList();
            Assert.That(statuses[0] == QuestionStatus.AnsweredCorrectly);
            Assert.That(statuses[1] == QuestionStatus.AnsweredBadly);
            Assert.That(statuses[2] == QuestionStatus.NotAnswered);
        }

        [Test]
        public void TrainingSetReturnsQuestions()
        {
            var q0 = _questionsSet.GetNextQuestion();
            _questionsSet.AnswerKnow();
            var q1 = _questionsSet.GetNextQuestion();
            _questionsSet.AnswerDontKnow();
            var q2 = _questionsSet.GetNextQuestion();
            _questionsSet.AnswerDontKnow();

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
                    _questionsSet.GetNextQuestion();
                    _questionsSet.GetNextQuestion();
                });
        }
    }
}