using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Xunit;
using Xunit.Abstractions;
using static Flashcards.SpacedRepetition.Leitner.Models;
using static Flashcards.SpacedRepetition.Leitner.Algorithm;

namespace LeitnerTests
{
	public class LeitnerTests
	{
		private const int FlashcardCount = 100;

		public LeitnerTests(ITestOutputHelper output)
		{
			Repository<Flashcard> flashcardRepository;

			void InitializeSpacedRepetitionModule()
			{
				var sqliteConnection =
					new DatabaseConnectionFactory()
						.CreateInMemoryConnection();

				_deckRepository = new Repository<Deck>(sqliteConnection);
				flashcardRepository = new Repository<Flashcard>(sqliteConnection);
				_cardDeckRepository = new Repository<CardDeck>(sqliteConnection);
				var tableCreator = new TableCreator(sqliteConnection);

				ISpacedRepetitionInitializer leitnerInitializer =
					new LeitnerInitializer(
						flashcardRepository,
						_deckRepository,
						_cardDeckRepository,
						tableCreator);
				leitnerInitializer.Initialize();
			}

			_output = output;
			InitializeSpacedRepetitionModule();

			var cards = Enumerable
				.Range(1, FlashcardCount)
				.Select(cardId =>
					new Flashcard
					{
						Id = cardId,
						LessonId = "1"
					});

			foreach (var card in cards)
			{
				flashcardRepository.Insert(card);
			}

			var repetitionDoneTodaySetting = new MockRepetitionTodaySetting();
			var sessionNumberSetting = new MockSessionNumberSetting();
			var streakDaysSetting = new MockStreakDaysSetting();
			_leitner = new LeitnerRepetition(
				_deckRepository,
				_cardDeckRepository,
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
		private Repository<CardDeck> _cardDeckRepository;

		private IEnumerable<Flashcard> Flashcards(string deck)
		{
			return _deckRepository
				.FindWhere(cd => cd.DeckTitle == deck)
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
				foreach (var deck in _deckRepository.GetAllWithChildren(null, true).Result)
					_output.WriteLine(deck.DeckTitle + ": " + deck.Cards.Count);
				_output.WriteLine("");
			}

			Assert.Equal(FlashcardCount, Flashcards(RetiredDeckTitle).Count());
			Assert.Equal(FlashcardCount, _leitner.LearnedFlashcards.Count());

		}

		[Fact]
		public async void AnsweringCorrectlyAllFlashcards_DecreasesNumberOfFlashcardsInTheNextSession()
		{
			var flashcards = _leitner.CurrentRepetitionFlashcards().Result;
			await _leitner.SubmitRepetitionResults(flashcards.Select(Known));
			_repetitionSession.Increment(); 
			
			var rearrangedFlashcards = _leitner.CurrentRepetitionFlashcards().Result;

			Assert.NotEqual(FlashcardCount, rearrangedFlashcards.Count());
		}

		[Fact]
		public async void AnsweringCorrectlyAllFlashcards_MovesThemToDeckBeginningWithSessionNumber()
		{
			var flashcards = _leitner.CurrentRepetitionFlashcards().Result;
			await _leitner.SubmitRepetitionResults(flashcards.Select(Known));

			var session0DeckCards =
				_deckRepository.FindWhere(cd => cd.DeckTitle == "0259").Result;
			Assert.NotEmpty(session0DeckCards);

			var currentDeckCards =
				_deckRepository.FindWhere(cd => cd.DeckTitle == CurrentDeckTitle).Result.Single().Cards;
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