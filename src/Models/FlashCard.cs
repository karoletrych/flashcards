using System;
using SQLite;

namespace Flashcards.Models
{
    public class Flashcard
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int LessonId { get; set; }

        [MaxLength(128)]
        public string Front { get; set; }

        [MaxLength(128)]
        public string Back { get; set; }

        [MaxLength(2000)]
        public string ImageUrl { get; set; }
    }
}