using System.Linq;
using FlashCards.Models;
using FlashCards.Services.Database;
using Xunit;
using DatabaseConnectionFactory = FlashCards.Services.Database.DatabaseConnectionFactory;

namespace FlashCards.UnitTests
{
    public class RepositoryTests
    {
        private readonly Repository<FlashCard> _flashCardRepository;
        private readonly Repository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var sqLiteAsyncConnection = DatabaseConnectionFactory.Connect(":memory:");
            _flashCardRepository = new Repository<FlashCard>(sqLiteAsyncConnection);
            _lessonRepository = new Repository<Lesson>(sqLiteAsyncConnection);
        }

        [Fact]
        public void RetrievalOfObjectsFromEmptyRepository()
        {
            var flashCards = _flashCardRepository.FindAll().Result;
            var lessons = _lessonRepository.FindAll().Result;

            Assert.Empty(flashCards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void InsertedObjectsAreRetrieved()
        {
            var lesson = new Lesson {TopLanguage = Language.English, BottomLanguage = Language.Polish};
            var flashCard = new FlashCard {Top = "cat", Bottom = "kot", LessonId = 1, Strength = 0.3m};
            _lessonRepository.Insert(lesson);
            _flashCardRepository.Insert(flashCard);

            var lessons = _lessonRepository.FindAll().Result;
            var flashCards = _flashCardRepository.FindAll().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashCards);
        }

        [Fact]
        public void ObjectsMatchingPredicateAreRetrieved()
        {
            var lesson = new Lesson { TopLanguage = Language.English, BottomLanguage = Language.Polish };
            var flashCard = new FlashCard { Top = "cat", Bottom = "kot", LessonId = 1, Strength = 0.3m };
            var flashCard2 = new FlashCard { Top = "dog", Bottom = "pies", LessonId = 1, Strength = 0.3m };
            _lessonRepository.Insert(lesson);
            _flashCardRepository.Insert(flashCard2);
            _flashCardRepository.Insert(flashCard);

            var matchingFlashCards = _flashCardRepository.FindMatching(f => f.LessonId == 1).Result;

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
            _lessonRepository.Insert(lesson);
            _lessonRepository.Insert(lesson);
            var lessons = _lessonRepository.FindAll().Result.ToList();
            Assert.Equal(1, lessons[0].Id);
            Assert.Equal(2, lessons[1].Id);
        }
    }
}