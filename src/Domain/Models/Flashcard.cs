using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Models
{
	public class Flashcard : IEquatable<Flashcard>
	{
		private const int MaxLength = 128;

		[PrimaryKey]
		public string Id { get; set; }

		[ForeignKey(typeof(Lesson))]

		// ReSharper disable once MemberCanBePrivate.Global required for OneToMany relationship with Lesson
		public string LessonId { get; set; }

		[MaxLength(MaxLength)]     
		public string Front { get; set; }

		[MaxLength(MaxLength)]
		public string Back { get; set; }

		[MaxLength(2000)]
		public string ImageUrl { get; set; }

		public DateTime Created { get; set; }

		public Flashcard()
		{

		}

		public static Flashcard CreateEmpty()
		{
			return new Flashcard{Id = Guid.NewGuid().ToString()};
		}

		public static Flashcard Create(string front, string back, string imageUri = null)
		{
			if (front.Length > MaxLength)
				throw new ArgumentException($"Front text length cannot be longer than {MaxLength}");
			if (back.Length > MaxLength)
				throw new ArgumentException($"Back text length cannot be longer than {MaxLength}");
			return new Flashcard
			{
				Id = Guid.NewGuid().ToString(),
				Front = front,
				Back = back,
				ImageUrl = imageUri,
				Created = DateTime.UtcNow
			};
		}

		public Flashcard Invert()
		{
			return new Flashcard{Id = Id, LessonId = LessonId, Front = Back, Back = Front, ImageUrl = ImageUrl};
		}

		public bool Equals(Flashcard other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Id, other.Id);
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
			return (Id != null
				? Id.GetHashCode()
				: 0);
		}
	}
}