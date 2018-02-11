using SQLite;

namespace FlashCards.Models.Dto
{
    public class Lesson
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public Language TopLanguage { get; set; }
        public Language BottomLanguage { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public int FlashCardCount { get; set; }
    }
}