using System;
using SQLite;

namespace FlashCards.Models.Dto
{
    public class FlashCard
    {
        private decimal _strength;

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int LessonId { get; set; }

        [MaxLength(128)]
        public string Top { get; set; }

        [MaxLength(128)]
        public string Bottom { get; set; }

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