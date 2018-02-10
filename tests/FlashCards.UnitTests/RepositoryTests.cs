using System.Runtime.InteropServices;
using FlashCards.Models;

namespace FlashCards.UnitTests
{
    public class RepositoryTests
    {
        private readonly Repository<FlashCard> _flashCardRepository;
        private readonly Repository<Lesson> _lessonRepository;

        public RepositoryTests()
        {
            var inMemoryConnection = DatabaseConnectionFactory.Connect(":memory:");
            _flashCardRepository = new Repository<FlashCard>(inMemoryConnection);
            _lessonRepository = new Repository<Lesson>(inMemoryConnection);
        }

        [Fact]
        public void RetrievalOfObjectsFromEmptyRepositorySucceeds()
        {
            var flashCards = _flashCardRepository.FindAll().Result;
            var lessons = _lessonRepository.FindAll().Result;

            Assert.Empty(flashCards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void InsertedObjectsCanBeRetrieved()
        {
            var lesson = new Lesson {TopLanguage = Language.English, BottomLanguage = Language.Polish};
            var flashCard = new FlashCard {Top = "cat", Bottom = "kot", LessonId = 1, Strength = 0.3m};
            _lessonRepository.Insert(lesson);
            _flashCardRepository.Insert(flashCard);

            var lessons = _lessonRepository.FindAll();
            var flashCards = _flashCardRepository.FindAll();

//            Assert.NotEmpty
        }
    }
}