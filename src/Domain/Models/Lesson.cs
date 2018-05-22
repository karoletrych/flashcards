using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Models
{
	public class Lesson
	{
		[PrimaryKey]
		public string Id { get; set; }

		// ReSharper disable once MemberCanBePrivate.Global
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();

		public Language FrontLanguage { get; set; }
		public Language BackLanguage { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }
		public AskingMode AskingMode { get; set; }
		public bool AskInRepetitions { get; set; }
		public bool Shuffle { get; set; }

		public Lesson()
		{
			
		}

		public static Lesson Create(Language frontLanguage, Language backLanguage, List<Flashcard> flashcards = null)
		{
			var lesson = new Lesson
			{
				Id = Guid.NewGuid().ToString(),
				FrontLanguage = frontLanguage,
				BackLanguage = backLanguage,
			};
			if (flashcards != null)
				lesson.Flashcards = flashcards;

			foreach (var flashcard in lesson.Flashcards)
			{
				flashcard.LessonId = lesson.Id;
			}

			return lesson;
		}

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