using Flashcards.Models;
using Flashcards.Services.Database;

namespace FlashCards.Services
{
    public class AddFlashcardService
    {
        private readonly IRepository<Flashcard> _flashcardRepository;

        public AddFlashcardService(IRepository<Flashcard> flashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
        }

        public void AddFlashcard(string frontText, string backText, int lessonId)
        {
            var flashcard = new Flashcard {Front = frontText, Back = backText, LessonId = lessonId, Strength = 0};
            _flashcardRepository.Insert(flashcard);
        }
    }
}