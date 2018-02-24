using System.Linq;
using Flashcards.Models;
using Flashcards.Services.Database;
using Xunit;
using DatabaseConnectionFactory = Flashcards.Services.Database.DatabaseConnectionFactory;

namespace Flashcards.UnitTests
{
    public class RepositoryTests
    {
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var sqliteConnection = DatabaseConnectionFactory.CreateConnection(":memory:");
            _flashcardRepository = new Repository<Flashcard>(sqliteConnection);
            _lessonRepository = new Repository<Lesson>(sqliteConnection);
        }

        [Fact]
        public void FindAll_EmptyRepository()
        {
            var flashcards = _flashcardRepository.FindAll().Result;
            var lessons = _lessonRepository.FindAll().Result;

            Assert.Empty(flashcards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void FindAll_ReturnsAllObjects()
        {
            var lesson = new Lesson {FrontLanguage = Language.English, BackLanguage = Language.Polish};
            var flashcard = new Flashcard {Front = "cat", Back = "kot", LessonId = 1, Strength = 0.3m};
            _lessonRepository.Insert(lesson);
            _flashcardRepository.Insert(flashcard);

            var lessons = _lessonRepository.FindAll().Result;
            var flashcards = _flashcardRepository.FindAll().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashcards);
        }

        [Fact]
        public void FindMatching_ReturnsResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish };
            var flashcard = new Flashcard { Front = "cat", Back = "kot", LessonId = 1, Strength = 0.3m };
            var flashcard2 = new Flashcard { Front = "dog", Back = "pies", LessonId = 1, Strength = 0.3m };
            _lessonRepository.Insert(lesson);
            _flashcardRepository.Insert(flashcard2);
            _flashcardRepository.Insert(flashcard);

            var matchingFlashcards = _flashcardRepository.FindMatching(f => f.LessonId == 1).Result;

            Assert.Equal(2, matchingFlashcards.Count());
        }

        [Fact]
        public void FindMatching_EmptyResults()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish };
            _lessonRepository.Insert(lesson);

            var matchingFlashcards = _flashcardRepository.FindMatching(f => f.LessonId == 1).Result;

            Assert.Empty(matchingFlashcards);
        }

        [Fact]
        public void OnInsert_IdIsIncremented()
        {
            var lesson = new Lesson
            {
                FrontLanguage = Language.English,
                BackLanguage = Language.Polish
            };
            _lessonRepository.Insert(lesson);
            _lessonRepository.Insert(lesson);
            var lessons = _lessonRepository.FindAll().Result.ToList();
            Assert.Equal(1, lessons[0].Id);
            Assert.Equal(2, lessons[1].Id);
        }
    }
}