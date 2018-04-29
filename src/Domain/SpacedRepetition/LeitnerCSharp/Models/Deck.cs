using System.Collections.Generic;
using Flashcards.Models;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Domain.SpacedRepetition.Leitner.Models
{
	public class Deck
	{
		[PrimaryKey]
		public int Id { get; set; } = 0;

		[Indexed]
		public string DeckTitle { get; set; } = DeckIds.CurrentDeckTitle;

		[ManyToMany(typeof(CardDeck), CascadeOperations = CascadeOperation.All)]
		public List<Flashcard> Cards { get; set; } = new List<Flashcard>();
	}
}