using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess.Database;
using Flashcards.SpacedRepetition.Provider;

namespace Flashcards.Services.DataAccess
{
    public class AddFlashcardService
    {
        private readonly IRepository<Flashcard> _flashcardRepository;
        private readonly IEnumerable<ISpacedRepetitionInitializer> _spacedRepetitionInitializers;

        public AddFlashcardService(
            IRepository<Flashcard> flashcardRepository, 
            IEnumerable<ISpacedRepetitionInitializer> spacedRepetitionInitializers)
        {
            _flashcardRepository = flashcardRepository;
            _spacedRepetitionInitializers = spacedRepetitionInitializers;
        }

        public async Task AddFlashcard(string frontText, string backText, int lessonId)
        {
            var flashcard = new Flashcard {Front = frontText, Back = backText, LessonId = lessonId};
            await _flashcardRepository.Insert(flashcard);
            foreach (var initializer in _spacedRepetitionInitializers)
            {
                initializer.Initialize(flashcard);
            }
        }
    }
}