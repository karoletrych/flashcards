using System.Linq;
using Flashcards.Models;
using System.Collections.Generic;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	static class Algorithm
	{
		public class MoveOperation
		{
			public Flashcard Card { get; set; }
			public string SourceDeck { get; set; }
			public string DestinationDeck { get; set; }
		}

		// https://en.wikipedia.org/wiki/Leitner_system
		public static IEnumerable<MoveOperation> RearrangeCards(IEnumerable<(Flashcard card, bool known, Deck deck)> repetitionResults, int sessionNumber)
		{
			return repetitionResults
				.Select(result =>
				{
					switch (result.deck.DeckTitle)
					{
						// If a learner is successful at a card from Deck Current,
						// it gets transferred into the progress deck that begins with 
						// that session's number.
						case var currentDeckTitle when result.known && currentDeckTitle == DeckTitles.CurrentDeckTitle:
							return new MoveOperation
							{
								Card = result.card,
								SourceDeck = DeckTitles.CurrentDeckTitle,
								DestinationDeck = DeckTitles.Titles
									.First(deck => deck.First().IsDigit(sessionNumber))
							};
						// If a learner has difficulty with a card during a subsequent review, 
						// the card is returned to Deck Current; 
						case var _ when !result.known:
							return new MoveOperation
							{
								Card = result.card,
								SourceDeck = result.deck.DeckTitle,
								DestinationDeck = DeckTitles.CurrentDeckTitle
							};
						// When a learner is successful at a card during a session that matches 
						// the last number on the deck that card goes into Deck Retired
						case var deckTitle when result.known && deckTitle.Last().ToInt() == sessionNumber:
							return new MoveOperation
							{
								Card = result.card,
								SourceDeck = result.deck.DeckTitle,
								DestinationDeck = DeckTitles.RetiredDeckTitle
							};
						default:
							return null;
					}
				})
				.Where(x => x != null);
		}
	}
}


				