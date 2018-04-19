using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Xunit;
using DatabaseConnectionFactory = Flashcards.Services.DataAccess.Database.DatabaseConnectionFactory;

namespace Flashcards.ServicesTests
{
    public class RepositoryTests
    {
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var sqliteConnection = new DatabaseConnectionFactory().CreateConnection(":memory:");
            _flashcardRepository = new Repository<Flashcard>(sqliteConnection);
            _lessonRepository = new Repository<Lesson>(sqliteConnection);
        }

        [Fact]
        public void FindAll_EmptyRepository()
        {
            var flashcards = _flashcardRepository.GetAllWithChildren(null, false).Result;
            var lessons = _lessonRepository.GetAllWithChildren(null, false).Result;

            Assert.Empty(flashcards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void FindAll_ReturnsAllObjects()
        {
            var lesson = new Lesson {FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1"};
            var flashcard = new Flashcard {Front = "cat", Back = "kot", LessonId = "1"};
            _lessonRepository.InsertWithChildren(lesson);
            _flashcardRepository.InsertWithChildren(flashcard);

            var lessons = _lessonRepository.GetAllWithChildren(null, false).Result;
            var flashcards = _flashcardRepository.GetAllWithChildren(null, false).Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashcards);
        }

        [Fact]
        public void FindMatching_ReturnsResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1" };
            var flashcard = new Flashcard { Front = "cat", Back = "kot", LessonId = "1"};
            var flashcard2 = new Flashcard { Front = "dog", Back = "pies", LessonId = "1" };
            _lessonRepository.InsertWithChildren(lesson);
            _flashcardRepository.InsertWithChildren(flashcard2);
            _flashcardRepository.InsertWithChildren(flashcard);

            var matchingFlashcards = _flashcardRepository.GetAllWithChildren(f => f.LessonId == "1", false).Result;

            Assert.Equal(2, matchingFlashcards.Count());
        }

        [Fact]
        public void FindMatching_EmptyResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish, Id = "1" };
            _lessonRepository.InsertWithChildren(lesson);

            var matchingFlashcards = _flashcardRepository.GetAllWithChildren(f => f.LessonId == "1", false).Result;

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
                    new Flashcard{Id = 0},
                    new Flashcard{Id = 1},
                    new Flashcard{Id = 2},
                },
	            Id = "1"
			};
            await _lessonRepository.InsertWithChildren(lesson);

            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren(null, false).Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren(null, false).Result);
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
                    new Flashcard{Id = 0},
                    new Flashcard{Id = 1},
                    new Flashcard{Id = 2},
                },
	            Id = "1"
			};
            await _lessonRepository.InsertWithChildren(lesson);
            var lessonRef = _lessonRepository.GetAllWithChildren(null, false).Result.Single();


            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren(null, false).Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren(null, false).Result);
        }
    }
}