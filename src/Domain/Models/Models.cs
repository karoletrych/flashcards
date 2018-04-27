using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Models
{
    public class Lesson
    {
        [PrimaryKey]
        public string Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Flashcard> Flashcards { get; set; }

        public Language FrontLanguage { get; set; }
        public Language BackLanguage { get; set; }

	    public AskingMode AskingMode { get; set; }
	    public bool AskInRepetitions { get; set; }
	    public bool Shuffle { get; set; }

	    protected bool Equals(Lesson other)
	    {
		    return string.Equals(Id, other.Id);
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((Lesson) obj);
	    }

	    public override int GetHashCode()
	    {
		    return (Id != null
			    ? Id.GetHashCode()
			    : 0);
	    }
    }

    public class Flashcard : IEquatable<Flashcard>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Lesson))]
        public string LessonId { get; set; }

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
		    return Id == other.Id;
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
		    return Id;
	    }
    }

	public enum AskingMode
	{
		Front,
		Back,
		Random
	}

	public enum Language
    {
	    English,
	    French,
		German,
	    Italian,
		Polish,
	    Norwegian,
	    Russian,
		Spanish,
        Swedish,
    }
}