using System.Collections.Generic;

namespace FlashCards.ViewModels.Lesson
{
    public class LessonListViewModel
    {
        public IEnumerable<LessonViewModel> Items { get; set; } =
            new List<LessonViewModel>
            {
                
            };
    }
}