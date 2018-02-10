using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite;

namespace FlashCards.Models
{
    public enum Language
    {
        German,
        English,
        Polish,
        French,
        Italian,
        Spanish,
        Swedish,
        Norwegian
    }

    public class Lesson
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public Language TopLanguage { get; set; }
        public Language BottomLanguage { get; set; }
    }

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