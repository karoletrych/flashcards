using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
		public string LessonId { get; set; }

		[MaxLength(MaxLength)]     
		public string Front { get; set; }

		[MaxLength(MaxLength)]
		public string Back { get; set; }

		[MaxLength(2000)]
		public string ImageUrl { get; set; }

		public Flashcard()
		{

		}

		public Flashcard(string lessonId, string front, string back, [CanBeNull] string imageUri)
		{
			if (front.Length > MaxLength)
				throw new ArgumentException($"Front text length cannot be longer than {MaxLength}");
			if (back.Length > MaxLength)
				throw new ArgumentException($"Back text length cannot be longer than {MaxLength}");

			Id = Guid.NewGuid().ToString();
			LessonId = lessonId;
			Front = front;
			Back = back;
			ImageUrl = imageUri;
		}

		public Flashcard Invert()
		{
			return new Flashcard(LessonId, Back, Front, ImageUrl);
		}

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
			var hashCode = 168429825;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LessonId);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Front);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Back);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ImageUrl);
			return hashCode;
		}
	}
}