using System.Collections.Generic;
using FlashCards.Models;

namespace FlashCards.ViewModels.Lesson
{
    public class LessonListViewModel
    {
        private readonly IRepository<Models.Dto.Lesson> _modelsRepository;

        public LessonListViewModel(IRepository<Models.Dto.Lesson> modelsRepository)
        {
            _modelsRepository = modelsRepository;
        }

        public IEnumerable<LessonViewModel> Items;
//            _modelsRepository.FindAll().Result.Select(l => new LessonViewModel(l));
    }
}