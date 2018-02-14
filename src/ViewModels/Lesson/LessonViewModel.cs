namespace FlashCards.ViewModels.Lesson
{
    public class LessonViewModel
    {
        public string Name { get; set; }
        public int FlashCardCount { get; set; }
        public string Languages { get; set; }


        public LessonViewModel(Models.Lesson lesson)
        {
            Name = lesson.Name;
            FlashCardCount = lesson.FlashCardCount;
            Languages = lesson.TopLanguage + " - " + lesson.BottomLanguage;
        }
    }
}