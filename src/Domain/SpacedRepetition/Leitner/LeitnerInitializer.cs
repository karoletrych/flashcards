﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	class LeitnerInitializer : ISpacedRepetitionInitializer
	{
		private readonly INotifyObjectInserted<Flashcard> _flashcardInsertedNotifier;
		private readonly IRepository<Deck> _deckRepository;
		private readonly IRepository<CardDeck> _cardDeckRepository;
		private readonly ITableCreator _tableCreator;

		public LeitnerInitializer(
			INotifyObjectInserted<Flashcard> flashcardInsertedNotifier,
			IRepository<Deck> deckRepository,
			IRepository<CardDeck> cardDeckRepository,
			ITableCreator tableCreator)
		{
			_flashcardInsertedNotifier = flashcardInsertedNotifier;
			_deckRepository = deckRepository;
			_cardDeckRepository = cardDeckRepository;
			_tableCreator = tableCreator;
		}

		private int DeckCurrentId => new Lazy<int>(() =>
		{
			var deck = _deckRepository.Single(d => d.DeckTitle == DeckTitles.CurrentDeckTitle).Result;
			return deck.Id;
		}).Value;

		public async Task InitializeAsync()
		{
			await _tableCreator.CreateTable<CardDeck>();
			await _tableCreator.CreateTable<Deck>();

			var decksCreated = await _deckRepository.Any();
			if (!decksCreated)
			{
				var decks = DeckTitles.Titles.AsEnumerable()
					.Select((title, id) =>
						new Deck {DeckTitle = title, Cards = new List<Flashcard>(), Id = id});
				await _deckRepository.InsertOrReplaceAllWithChildren(decks);
			}

			_flashcardInsertedNotifier.ObjectInserted +=
				(f, flashcard) =>
				{
					var cardDeck = new CardDeck {CardId = flashcard.Id, DeckId = DeckCurrentId};
					_cardDeckRepository.Insert(cardDeck);
				};
		}
	}
}

