using System.Collections.Generic;
using FlashCards.Services.Database;

namespace FlashCards.ViewModels.Lesson
{
    public class LessonListViewModel
    {
        private readonly IRepository<Models.Lesson> _modelsRepository;

        public LessonListViewModel(IRepository<Models.Lesson> modelsRepository)
        {
            _modelsRepository = modelsRepository;
        }

        public IEnumerable<LessonViewModel> Items;// =
//            _modelsRepository.FindAll().Result.Select(l => new LessonViewModel(l));
    }
}