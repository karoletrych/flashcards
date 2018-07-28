using System.Collections.Generic;
using System.Linq;
using Flashcards.Domain.SpacedRepetition.Leitner;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;
using FluentAssertions;
using Settings;
using Xunit;
using Xunit.Abstractions;

namespace LeitnerTests
{
	public class LeitnerTests
	{
		private const int FlashcardCount = 100;

		public LeitnerTests(ITestOutputHelper output)
		{
			Repository<CardDeck> cardDeckRepository;
			Repository<Flashcard> flashcardRepository;

			void InitializeSpacedRepetitionModule()
			{
				var connection =
					new Connection(new DatabaseConnectionFactory()
						.CreateInMemoryConnection());

				_deckRepository = new Repository<Deck>(() =>connection);
				flashcardRepository = new Repository<Flashcard>(() => connection);
				cardDeckRepository = new Repository<CardDeck>(() => connection);

				ISpacedRepetitionInitializer leitnerInitializer =
					new LeitnerInitializer(
						flashcardRepository,
						_deckRepository,
						cardDeckRepository,
						connection);
				leitnerInitializer.InitializeAsync();
			}

			_output = output;
			InitializeSpacedRepetitionModule();

			var cards = Enumerable
				.Range(1, FlashcardCount)
				.Select(_ =>
					Flashcard.Create("A", "B"));

			foreach (var flashcard in cards)
			{
				flashcardRepository.Insert(flashcard).Wait();
			}

			var repetitionDoneTodaySetting = new MockRepetitionTodaySetting();
			var sessionNumberSetting = new MockSessionNumberSetting();
			var streakDaysSetting = new MockStreakDaysSetting();
			_leitner = new LeitnerRepetition(
				_deckRepository,
				cardDeckRepository,
				sessionNumberSetting,
				repetitionDoneTodaySetting,
				streakDaysSetting);

			_repetitionSession = new RepetitionSession(repetitionDoneTodaySetting, sessionNumberSetting, streakDaysSetting);
		}

		private class MockSessionNumberSetting : ISetting<int>
		{
			public int Value { get; set; }
		}

		private class MockRepetitionTodaySetting : ISetting<bool>
		{
			public bool Value { get; set; }
		}

		private class MockStreakDaysSetting : ISetting<int>
		{
			public int Value { get; set; }
		}

		private readonly ITestOutputHelper _output;

		private Repository<Deck> _deckRepository;
		private readonly ISpacedRepetition _leitner;
		private readonly IRepetitionSession _repetitionSession;

		private IEnumerable<Flashcard> Flashcards(string deck)
		{
			return _deckRepository
				.GetWithChildren(cd => cd.DeckTitle == deck)
				.Result
				.Single()
				.Cards;
		}

		private static QuestionResult Known(Flashcard f)
		{
			return new QuestionResult(f, true);
		}

		[Fact]
		public async void After10CorrectSessions_AllCardsAreInRetiredDeck()
		{
			for (var i = 0; i < 20; ++i)
			{
				var flashcards = _leitner.CurrentRepetitionFlashcards().Result.ToList();
				await _leitner.SubmitRepetitionResults(flashcards.Select(Known));
				_repetitionSession.Increment();
				_output.WriteLine($"session: {i}");
				foreach (var deck in _deckRepository.GetAllWithChildren().Result)
					_output.WriteLine(deck.DeckTitle + ": " + deck.Cards.Count);
				_output.WriteLine("");
			}


			Assert.Equal(FlashcardCount, Flashcards(DeckTitles.RetiredDeckTitle).Count());
			Assert.Equal(FlashcardCount, (await _leitner.LearnedFlashcards()).Count());
		}

		[Fact]
		public async void AnsweringCorrectlyAllFlashcards_DecreasesNumberOfFlashcardsInTheNextSession()
		{
			var flashcards = _leitner.CurrentRepetitionFlashcards().Result;
			await _leitner.SubmitRepetitionResults(flashcards.Select(Known));
			_repetitionSession.Increment(); 
			
			var rearrangedFlashcards = _leitner.CurrentRepetitionFlashcards().Result;

			rearrangedFlashcards.Count().Should().Be(0);
		}


		[Fact]
		public async void AnsweringCorrectlyAllFlashcards_MovesThemToDeckBeginningWithSessionNumber()
		{
			var flashcards = _leitner.CurrentRepetitionFlashcards().Result;
			await _leitner.SubmitRepetitionResults(flashcards.Select(Known));

			var session0DeckCards =
				_deckRepository.GetWithChildren(cd => cd.DeckTitle == "0259").Result;
			Assert.NotEmpty(session0DeckCards);

			var currentDeckCards =
				_deckRepository.GetWithChildren(cd => cd.DeckTitle == DeckTitles.CurrentDeckTitle)
					.Result.Single().Cards;
			Assert.Empty(currentDeckCards);
		}

		[Fact]
		public void ChooseFlashcards_ReturnsAllFromCurrentDeck()
		{
			var flashcards = _leitner.CurrentRepetitionFlashcards().Result;
			Assert.Equal(FlashcardCount, flashcards.Count());
		}
	}
}