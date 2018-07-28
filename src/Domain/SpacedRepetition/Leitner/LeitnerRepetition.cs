using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;
using Settings;

[assembly: InternalsVisibleTo("LeitnerTests")]

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	internal class LeitnerRepetition : ISpacedRepetition
	{
		private readonly IRepository<CardDeck> _cardDeckRepository;
		private readonly IRepository<Deck> _deckRepository;
		private readonly ISetting<bool> _repetitionDoneTodaySetting;
		private readonly ISetting<int> _sessionNumberSetting;
		private readonly ISetting<int> _streakDaysSetting;

		public LeitnerRepetition(
			IRepository<Deck> deckRepository,
			IRepository<CardDeck> cardDeckRepository,
			ISetting<int> sessionNumberSetting,
			ISetting<bool> repetitionDoneTodaySetting,
			ISetting<int> streakDaysSetting)
		{
			_deckRepository = deckRepository;
			_cardDeckRepository = cardDeckRepository;
			_sessionNumberSetting = sessionNumberSetting;
			_repetitionDoneTodaySetting = repetitionDoneTodaySetting;
			_streakDaysSetting = streakDaysSetting;
		}

		public async Task<IEnumerable<Flashcard>> CurrentRepetitionFlashcards()
		{
			if (_repetitionDoneTodaySetting.Value)
				return Enumerable.Empty<Flashcard>();
			var sessionNumber = _sessionNumberSetting.Value;
			var decks = await _deckRepository.GetAllWithChildren();

			var result =
				decks
					.Where(deck =>
						deck.DeckTitle.Select(c => c.ToInt()).Contains(sessionNumber) ||
						deck.DeckTitle == DeckTitles.CurrentDeckTitle)
					.SelectMany(deck => deck.Cards);
			return result;
		}

		public async Task<IEnumerable<Flashcard>> LearnedFlashcards()
		{
			var deck = await _deckRepository.SingleWithChildren(d => d.DeckTitle == DeckTitles.RetiredDeckTitle);
			return deck.Cards;
		}

		public async Task SubmitRepetitionResults(IEnumerable<QuestionResult> results)
		{
			var questionResults = results.ToList();
			var cardsWithDecks = await Task.WhenAll(questionResults.Select(async result =>
				(result.Flashcard, result.IsKnown, await ParentDeck(result.Flashcard.Id))));

			var decksMoveOperations = Algorithm.RearrangeCards(cardsWithDecks, _sessionNumberSetting.Value);

			var decksMap = (await _deckRepository.GetAllWithChildren())
				.ToDictionary(deck => deck.DeckTitle, deck => deck.Id);

			foreach (var moveOperation in decksMoveOperations)
			{
				var destinationId = decksMap[moveOperation.DestinationDeck];
				var toRemove = await _cardDeckRepository
					.Single(cd => cd.CardId == moveOperation.Card.Id);
				await _cardDeckRepository
					.Delete(toRemove);
				await _cardDeckRepository
					.Insert(new CardDeck
					{
						CardId = moveOperation.Card.Id,
						DeckId = destinationId
					});
			}

			_repetitionDoneTodaySetting.Value = true;
			_streakDaysSetting.Value++;
		}

		private async Task<Deck> ParentDeck(string flashcardId)
		{
			var cardDeck = await _cardDeckRepository.Single(cd => cd.CardId == flashcardId);
			var deck = await _deckRepository.Single(d => d.Id == cardDeck.DeckId);
			return deck;
		}
	}
}