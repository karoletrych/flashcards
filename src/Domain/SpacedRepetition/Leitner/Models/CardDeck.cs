using Flashcards.Models;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flashcards.Domain.SpacedRepetition.Leitner.Models
{
	public class CardDeck
	{
		[ForeignKey(typeof(Flashcard))]
		public string CardId { get; set; }

		[ForeignKey(typeof(Deck))]
		public int DeckId { get; set; } = 0;

		[PrimaryKey]
		[AutoIncrement]
		public int CardDeckId { get; set; } = 0;
	}
}