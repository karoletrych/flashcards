using System;
using SQLite;

namespace Flashcards.Models
{
    public class Flashcard
    {
        private decimal _strength;

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

        public decimal Strength
        {
            get => _strength;
            set
            {
                if (value > 1 || value < 0)
                    throw new ArgumentException($"Invalid value of strength: {value}");
                _strength = value;
            }
        }
    }
}