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
            var sqliteConnection = new Connection(new DatabaseConnectionFactory().CreateInMemoryConnection());
            _flashcardRepository = new Repository<Flashcard>(() => sqliteConnection);
            _lessonRepository = new Repository<Lesson>(() => sqliteConnection);
        }

        [Fact]
        public void FindAll_EmptyRepository()
        {
            var flashcards = _flashcardRepository.GetAllWithChildren().Result;
            var lessons = _lessonRepository.GetAllWithChildren().Result;

            Assert.Empty(flashcards);
            Assert.Empty(lessons);
        }

        [Fact]
        public void FindAll_ReturnsAllObjects()
        {
	        var lesson = Lesson.Create(Language.English, Language.Polish, new List<Flashcard>());
            var flashcard = Flashcard.Create("cat", "kot");

			_lessonRepository.Insert(lesson);
            _flashcardRepository.Insert(flashcard);

            var lessons = _lessonRepository.GetAllWithChildren().Result;
            var flashcards = _flashcardRepository.GetAllWithChildren().Result;

            Assert.NotEmpty(lessons);
            Assert.NotEmpty(flashcards);
        }

        [Fact]
        public async void FlashcardsAreDeleted_WhenItsLessonIsDeleted()
        {
            var lesson = Lesson.Create(Language.English, Language.Polish,
				new List<Flashcard>
                {
                    Flashcard.CreateEmpty(),
                    Flashcard.CreateEmpty(),
                    Flashcard.CreateEmpty(),
                }
			);
            await _lessonRepository.Insert(lesson);

            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren().Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren().Result);
        }

        [Fact]
        public async void FlashcardsAreDeleted_WhenItsLessonIsDeletedBySeparateReference()
        {
            var lesson = Lesson.Create(Language.English, Language.Polish,
                new List<Flashcard>
                {
	                Flashcard.CreateEmpty(),
	                Flashcard.CreateEmpty(),
	                Flashcard.CreateEmpty(),
                }
			);
            await _lessonRepository.Insert(lesson);
            var lessonRef = _lessonRepository.GetAllWithChildren().Result.Single();

            await _lessonRepository.Delete(lesson);
            Assert.Empty(_lessonRepository.GetAllWithChildren().Result);
            Assert.Empty(_flashcardRepository.GetAllWithChildren().Result);
        }
    }
}