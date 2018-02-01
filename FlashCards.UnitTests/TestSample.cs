using System;
using FlashCards.Model;
using NUnit.Framework;


namespace FlashCards.UnitTests
{
    [TestFixture]
    public class TrainingSetTests
    {
        private TrainingSet _trainingSet;

        [SetUp]
        public void Setup()
        {
            _trainingSet = new TrainingSet();
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