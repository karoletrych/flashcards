using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Database;

namespace FlashCards.Services
{
    public class ExaminerFactory
    {
        private readonly IRepository<Flashcard> _flashcardRepository;

        public ExaminerFactory(IRepository<Flashcard> flashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
        }

        public async Task<Examiner> Create(int lessonId)
        {
            var lessonsFlashcards =
                await _flashcardRepository.FindMatching(flashcard => flashcard.LessonId == lessonId);
            var questions =
                lessonsFlashcards
                    .Select(flashcard => new FlashcardQuestion(flashcard.Front, flashcard.Back))
                    .ToList();

            return new Examiner(questions);
        }
    }
}