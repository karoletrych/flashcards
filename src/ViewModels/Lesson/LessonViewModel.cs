namespace Flashcards.ViewModels.Lesson
{
    public class LessonViewModel
    {
        public string Name { get; set; }
        public string Languages { get; set; }

        public LessonViewModel(Models.Lesson lesson)
        {
            Name = lesson.Name;
            Languages = lesson.FrontLanguage + " - " + lesson.BackLanguage;
        }
    }
}