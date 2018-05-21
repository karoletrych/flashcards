using System.Collections.Generic;
using System.Linq;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class RepositoryTests
    {
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var sqliteConnection = new Connection(new DatabaseConnectionFactory().CreateConnection(":memory:"));
            _flashcardRepository = new Repository<Flashcard>(() => sqliteConnection);
            _lessonRepository = new Repository<Lesson>(() => sqliteConnection);
        }

        [Fact]
        public void FindAll_EmptyRepository()
        {
            var flashcards = _flashcardRepository.GetWithChildren(null).Result;
            var lessons = _lessonRepository.GetWithChildren(null).Result;

            Assert.Empty(flashcards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void FindAll_ReturnsAllObjects()
        {
            var lesson = new Lesson {FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1"};
            var flashcard = new Flashcard ("1", "cat", "kot");
            _lessonRepository.Insert(lesson);
            _flashcardRepository.Insert(flashcard);

            var lessons = _lessonRepository.GetAllWithChildren().Result;
            var flashcards = _flashcardRepository.GetAllWithChildren().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashcards);
        }

        [Fact]
        public void FindMatching_ReturnsResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1" };
	        var flashcard = new Flashcard("1", "cat", "kot");
            var flashcard2 = new Flashcard("1", "dog", "pies");
            _lessonRepository.Insert(lesson);
            _flashcardRepository.Insert(flashcard2);
            _flashcardRepository.Insert(flashcard);

            var matchingFlashcards = _flashcardRepository.GetWithChildren(f => f.LessonId == "1").Result;

            Assert.Equal(2, matchingFlashcards.Count());
        }

        [Fact]
        public void FindMatching_EmptyResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1" };
            _lessonRepository.Insert(lesson);

            var matchingFlashcards = _flashcardRepository.GetWithChildren(f => f.LessonId == "1").Result;

            Assert.Empty(matchingFlashcards);
        }

        [Fact]
        public async void FlashcardsAreDeleted_WhenItsLessonIsDeleted()
        {
            var lesson = new Lesson
            {
                FrontLanguage = Language.English,
                BackLanguage = Language.Polish,
                Flashcards = new List<Flashcard>
                {
                    new Flashcard{Id = "0"},
                    new Flashcard{Id = "1"},
                    new Flashcard{Id = "2"},
                },
	            Id = "1"
			};
            await _lessonRepository.Insert(lesson);

            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren().Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren().Result);
        }

        [Fact]
        public async void FlashcardsAreDeleted_WhenItsLessonIsDeletedBySeparateReference()
        {
            var lesson = new Lesson
            {
                FrontLanguage = Language.English,
                BackLanguage = Language.Polish,
                Flashcards = new List<Flashcard>
                {
                    new Flashcard{Id = "0"},
                    new Flashcard{Id = "1"},
                    new Flashcard{Id = "2"},
                },
	            Id = "1"
			};
            await _lessonRepository.Insert(lesson);
            var lessonRef = _lessonRepository.GetAllWithChildren().Result.Single();

            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren().Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren().Result);
        }
    }
}