using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Models
{
    public class Lesson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Flashcard> Flashcards { get; set; }

        public Language FrontLanguage { get; set; }
        public Language BackLanguage { get; set; }
    }

    public class Flashcard : IEquatable<Flashcard>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Lesson))]
        public int LessonId { get; set; }

        [MaxLength(128)]     
        public string Front { get; set; }

        [MaxLength(128)]
        public string Back { get; set; }

        [MaxLength(2000)]
        public string ImageUrl { get; set; }

        public bool Equals(Flashcard other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && LessonId == other.LessonId && string.Equals(Front, other.Front) && string.Equals(Back, other.Back) && string.Equals(ImageUrl, other.ImageUrl);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Flashcard) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ LessonId;
                hashCode = (hashCode * 397) ^ (Front != null ? Front.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Back != null ? Back.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public enum Language
    {
        German,
        English,
        Polish,
        French,
        Italian,
        Spanish,
        Swedish,
        Norwegian,
        Russian
    }
}