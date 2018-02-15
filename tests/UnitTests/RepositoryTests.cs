using System.Linq;
using FlashCards.Models;
using FlashCards.Services.Database;
using Xunit;
using DatabaseConnectionFactory = FlashCards.Services.Database.DatabaseConnectionFactory;

namespace FlashCards.UnitTests
{
    public class RepositoryTests
    {
        private readonly IRepository<FlashCard> _flashCardAsyncRepository;
        private readonly IRepository<Lesson> _lessonAsyncRepository;

        public RepositoryTests()
        {
            var sqliteConnection = DatabaseConnectionFactory.CreateConnection(":memory:");
            _flashCardAsyncRepository = new Repository<FlashCard>(sqliteConnection);
            _lessonAsyncRepository = new Repository<Lesson>(sqliteConnection);
        }

        [Fact]
        public void RetrievalOfObjectsFromEmptyRepository()
        {
            var flashCards = _flashCardAsyncRepository.FindAll().Result;
            var lessons = _lessonAsyncRepository.FindAll().Result;

            Assert.Empty(flashCards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void InsertedObjectsAreRetrieved()
        {
            var lesson = new Lesson {TopLanguage = Language.English, BottomLanguage = Language.Polish};
            var flashCard = new FlashCard {Top = "cat", Bottom = "kot", LessonId = 1, Strength = 0.3m};
            _lessonAsyncRepository.Insert(lesson);
            _flashCardAsyncRepository.Insert(flashCard);

            var lessons = _lessonAsyncRepository.FindAll().Result;
            var flashCards = _flashCardAsyncRepository.FindAll().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashCards);
        }

        [Fact]
        public void ObjectsMatchingPredicateAreRetrieved()
        {
            var lesson = new Lesson { TopLanguage = Language.English, BottomLanguage = Language.Polish };
            var flashCard = new FlashCard { Top = "cat", Bottom = "kot", LessonId = 1, Strength = 0.3m };
            var flashCard2 = new FlashCard { Top = "dog", Bottom = "pies", LessonId = 1, Strength = 0.3m };
            _lessonAsyncRepository.Insert(lesson);
            _flashCardAsyncRepository.Insert(flashCard2);
            _flashCardAsyncRepository.Insert(flashCard);

            var matchingFlashCards = _flashCardAsyncRepository.FindMatching(f => f.LessonId == 1).Result;

            Assert.Equal(2, matchingFlashCards.Count());
        }

        [Fact]
        public void IdIsIncremented()
        {
            var lesson = new Lesson
            {
                TopLanguage = Language.English,
                BottomLanguage = Language.Polish
            };
            _lessonAsyncRepository.Insert(lesson);
            _lessonAsyncRepository.Insert(lesson);
            var lessons = _lessonAsyncRepository.FindAll().Result.ToList();
            Assert.Equal(1, lessons[0].Id);
            Assert.Equal(2, lessons[1].Id);
        }
    }
}