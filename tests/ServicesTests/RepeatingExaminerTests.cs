using System.Collections.Generic;
using Flashcards.Models;
using Flashcards.Services.Examiner;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class RepeatingExaminerTests
    {
        public RepeatingExaminerTests()
        {
            var questions = new List<Flashcard>
            {
                new Flashcard {Id = 1, Front = "dog", Back = "pies"},
                new Flashcard {Id = 2, Front = "cat", Back = "kot"}
            };
            _examiner = new RepeatingExaminer(questions, new ExaminerBuilder());
        }

        private readonly RepeatingExaminer _examiner;

        [Fact]
        public void RepeatsUnknownQuestions()
        {
            _examiner.TryAskNextQuestion(out var failedQuestion);
            _examiner.Answer(false);
            _examiner.TryAskNextQuestion(out var _);
            _examiner.Answer(true);

            _examiner.TryAskNextQuestion(out var repeatedQuestion);

            Assert.Equal(
                failedQuestion.Id,
                repeatedQuestion.Id);


            _examiner.Answer(true);
        }
    }
}