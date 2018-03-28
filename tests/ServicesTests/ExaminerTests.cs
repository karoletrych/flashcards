using System;
using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class ExaminerTests
    {
        public ExaminerTests()
        {
            _examiner = new Examiner(_questions);
        }

        private readonly Examiner _examiner;

        private readonly IList<Flashcard> _questions = new[]
        {
            new Flashcard {Front = "dog", Back = "pies"},
            new Flashcard {Front = "cat", Back = "kot"},
            new Flashcard {Front = "duck", Back = "kaczka"}
        };

        [Fact]
        public void AnsweringTwiceSameQuestion_CausesException()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    _examiner.TryAskNextQuestion(out _);
                    _examiner.Answer(true);
                    _examiner.Answer(false);
                });
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
        public void QuestionResultsIsSet_WhenAllQuestionsAreAnswered()
        {
            var raisedEvent = Assert.Raises<QuestionResultsEventArgs>(
                h => _examiner.QuestionsAnswered += h,
                h => _examiner.QuestionsAnswered -= h,
                () =>
                {
                    _examiner.TryAskNextQuestion(out var _);
                    _examiner.Answer(true);
                    _examiner.TryAskNextQuestion(out var _);
                    _examiner.Answer(false);
                    _examiner.TryAskNextQuestion(out var _);
                    _examiner.Answer(false);
                    _examiner.TryAskNextQuestion(out var _);
                });

            var questionResults = raisedEvent.Arguments.Results.ToList();
            Assert.True(questionResults[0].IsKnown);
            Assert.False(questionResults[1].IsKnown);
            Assert.False(questionResults[2].IsKnown);
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
    }
}