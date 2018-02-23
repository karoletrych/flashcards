using System.Linq;
using Flashcards.Models;
using Flashcards.Services.Database;
using Xunit;
using DatabaseConnectionFactory = Flashcards.Services.Database.DatabaseConnectionFactory;

namespace Flashcards.UnitTests
{
    public class RepositoryTests
    {
        private readonly IRepository<Flashcard> _FlashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var sqliteConnection = DatabaseConnectionFactory.CreateConnection(":memory:");
            _FlashcardRepository = new Repository<Flashcard>(sqliteConnection);
            _lessonRepository = new Repository<Lesson>(sqliteConnection);
        }

        [Fact]
        public void RetrievalOfObjectsFromEmptyRepository()
        {
            var Flashcards = _FlashcardRepository.FindAll().Result;
            var lessons = _lessonRepository.FindAll().Result;

            Assert.Empty(Flashcards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void InsertedObjectsAreRetrieved()
        {
            var lesson = new Lesson {FrontLanguage = Language.English, BackLanguage = Language.Polish};
            var Flashcard = new Flashcard {Front = "cat", Back = "kot", LessonId = 1, Strength = 0.3m};
            _lessonRepository.Insert(lesson);
            _FlashcardRepository.Insert(Flashcard);

            var lessons = _lessonRepository.FindAll().Result;
            var Flashcards = _FlashcardRepository.FindAll().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(Flashcards);
        }

        [Fact]
        public void ObjectsMatchingPredicateAreRetrieved()
        {
            var lesson = new Lesson { FrontLanguage = Language.English, BackLanguage = Language.Polish };
            var Flashcard = new Flashcard { Front = "cat", Back = "kot", LessonId = 1, Strength = 0.3m };
            var Flashcard2 = new Flashcard { Front = "dog", Back = "pies", LessonId = 1, Strength = 0.3m };
            _lessonRepository.Insert(lesson);
            _FlashcardRepository.Insert(Flashcard2);
            _FlashcardRepository.Insert(Flashcard);

            var matchingFlashcards = _FlashcardRepository.FindMatching(f => f.LessonId == 1).Result;

            Assert.Equal(2, matchingFlashcards.Count());
        }

        [Fact]
        public void IdIsIncremented()
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