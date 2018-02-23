using System;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.Database;

namespace Flashcards.Services
{
    public class AddLessonService
    {
        private readonly IRepository<Lesson> _lessonRepository;

        public AddLessonService(IRepository<Lesson> lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<int> AddLesson(string name, Language frontLanguage, Language backLanguage)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Lesson name cannot be empty");
            var lesson = new Lesson
            {
                Name = name,
                FrontLanguage = frontLanguage,
                BackLanguage = backLanguage,
            };
            return await _lessonRepository.Insert(lesson);
        }
    }
}