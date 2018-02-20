using SQLite;

namespace FlashCards.Models
{
    public class Lesson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Language FrontLanguage { get; set; }
        public Language BackLanguage { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public int FlashCardCount { get; set; }
    }
}