using System;
using System.Threading.Tasks;
using FlashCards.Models;
using FlashCards.Services.Database;

namespace FlashCards.Services
{
    class AddLessonService
    {
        private readonly IRepository<Lesson> _lessonRepository;

        public AddLessonService(IRepository<Lesson> lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<int> Add(string name, Language frontLanguage, Language backLanguage)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Lesson name cannot be empty");
            var lesson = new Lesson
            {
                Name = name,
                FrontLanguage = frontLanguage,
                BackLanguage = backLanguage,
                FlashCardCount = 0
            };
            return await _lessonRepository.Insert(lesson);
        }
    }
}