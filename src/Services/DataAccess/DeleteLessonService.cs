using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess.Database;

namespace Flashcards.Services.DataAccess
{
    public class DeleteLessonService
    {
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public DeleteLessonService(IRepository<Flashcard> flashcardRepository,
            IRepository<Lesson> lessonRepository)
        {
            _flashcardRepository = flashcardRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task Delete(int lessonId)
        {
            var flashcards = await _flashcardRepository
                .FindMatching(flashcard => flashcard.LessonId == lessonId);
            await Task.WhenAll(
                flashcards.Select(flashcard => _flashcardRepository.Delete(flashcard.Id)));

            await _lessonRepository.Delete(lessonId);
        }
    }
}