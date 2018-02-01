using System;
using System.Collections.Generic;
using System.Linq;
using FlashCards.Model;
using FlashCards.ViewModel;
using NUnit.Framework;

namespace FlashCards.UnitTests
{
    [TestFixture]
    public class TrainingSetTests
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
            _trainingSet = new TrainingSet(_questions);
        }

        private TrainingSet _trainingSet;
        private IList<Question> _questions;

        [Test]
        public void QuestionsStatusesCorrespondToAnswers()
        {
            _trainingSet.GetNextQuestion();
            _trainingSet.AnswerKnow();
            _trainingSet.GetNextQuestion();
            _trainingSet.AnswerDontKnow();

            var statuses = _trainingSet.QuestionsStatuses.ToList();
            Assert.That(statuses[0] == QuestionStatus.AnsweredCorrectly);
            Assert.That(statuses[1] == QuestionStatus.AnsweredWrongly);
            Assert.That(statuses[2] == QuestionStatus.NotAnswered);
        }

        [Test]
        public void TrainingSetReturnsQuestions()
        {
            var q0 = _trainingSet.GetNextQuestion();
            _trainingSet.AnswerKnow();
            var q1 = _trainingSet.GetNextQuestion();
            _trainingSet.AnswerDontKnow();
            var q2 = _trainingSet.GetNextQuestion();
            _trainingSet.AnswerDontKnow();

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
                    _trainingSet.GetNextQuestion();
                    _trainingSet.GetNextQuestion();
                });
        }
    }
}