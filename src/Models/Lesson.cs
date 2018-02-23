using SQLite;

namespace Flashcards.Models
{
    public class Lesson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public Language FrontLanguage { get; set; }
        public Language BackLanguage { get; set; }
    }
}