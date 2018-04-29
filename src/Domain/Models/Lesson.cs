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
}