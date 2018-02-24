using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Database;

namespace FlashCards.Services
{
    public interface IExaminerModelFactory
    {
        Task<ExaminerModel> Create(int lessonId);
    }

    public class ExaminerModelFactory : IExaminerModelFactory
    {
        private readonly IRepository<Flashcard> _flashcardRepository;

        public ExaminerModelFactory(IRepository<Flashcard> flashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
        }

        public async Task<ExaminerModel> Create(int lessonId)
        {
            var lessonsFlashcards =
                await _flashcardRepository.FindMatching(flashcard => flashcard.LessonId == lessonId);
            var questions =
                lessonsFlashcards
                    .Select(flashcard => new FlashcardQuestion(flashcard.Front, flashcard.Back));

            return new ExaminerModel(questions);
        }
    }
}