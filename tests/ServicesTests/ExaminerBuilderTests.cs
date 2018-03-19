using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class ExaminerBuilderTests
    {
        private readonly ExaminerBuilder _examinerBuilder;

        public ExaminerBuilderTests()
        {
            _examinerBuilder = new ExaminerBuilder();
        }
        
        [Fact]
        public void CreatesExaminerWithFlashcards()
        {
            var flashcards = new[] {new Flashcard { Id = 1}, new Flashcard{Id = 2},};
            
            var examiner = _examinerBuilder
                .WithFlashcards(flashcards)
                .Build();
            
            Assert.Equal(flashcards.Length, examiner.Questions.Count());
        }
    }
}