using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.DataAccess.Database;
using Flashcards.SpacedRepetition.Interface;
using Xunit;
using Xunit.Abstractions;
using static Flashcards.SpacedRepetition.Leitner.Models;
using static Flashcards.SpacedRepetition.Leitner.Algorithm;

namespace LeitnerTests
{
	public class LeitnerTests
	{
		public LeitnerTests(ITestOutputHelper output)
		{
			_output = output;
			var flashcardRepository = InitializeSpacedRepetitionModule();

			Enumerable
				.Range(1, FlashcardCount)
				.Select(cardId =>
					new Flashcard
					{
						Id = cardId,
						LessonId = "1"
					})
				.ToList()
				.ForEach(f => flashcardRepository.Insert(f));

			_leitner = new LeitnerRepetition(
				_deckRepository);
		}

		[Fact]
		public void After10CorrectSessions_AllCardsAreInRetiredDeck()
		{
			for (var i = 0; i < 20; ++i)
			{
				var flashcards = _leitner.ChooseFlashcards(i).Result.ToList();
				_leitner.RearrangeFlashcards(flashcards.Select(Known), i);
				_output.WriteLine($"session: {i}");
				foreach (var deck in _deckRepository.FindAll().Result)
					_output.WriteLine(deck.DeckTitle + ": " + deck.Cards.Count());
				_output.WriteLine("");
			}

			Assert.Equal(FlashcardCount, Flashcards(RetiredDeckTitle).Count());
		}

		[Fact]
		public void AnsweringCorrectlyAllFlashcards_DecreasesNumberOfFlashcardsInTheNextSession()
		{
			var flashcards = _leitner.ChooseFlashcards(0).Result;
			_leitner.RearrangeFlashcards(flashcards.Select(Known), 0);
			var rearrangedFlashcards = _leitner.ChooseFlashcards(1).Result;

			Assert.NotEqual(FlashcardCount, rearrangedFlashcards.Count());
		}

		[Fact]
		public void AnsweringCorrectlyAllFlashcards_MovesThemToDeckBeginningWithSessionNumber()
		{
			var flashcards = _leitner.ChooseFlashcards(0).Result;
			_leitner.RearrangeFlashcards(flashcards.Select(Known), 0);

			var session0DeckCards =
				_deckRepository.FindMatching(cd => cd.DeckTitle == "0259").Result;
			Assert.NotEmpty(session0DeckCards);

			var currentDeckCards =
				_deckRepository.FindMatching(cd => cd.DeckTitle == CurrentDeckTitle).Result.Single().Cards;
			Assert.Empty(currentDeckCards);
		}

		[Fact]
		public void ChooseFlashcards_ReturnsAllFromCurrentDeck()
		{
			var flashcards = _leitner.ChooseFlashcards(0).Result;
			Assert.Equal(FlashcardCount, flashcards.Count());
		}

		private const int FlashcardCount = 20;

		private Repository<Flashcard> InitializeSpacedRepetitionModule()
		{
			var sqliteConnection =
				new DatabaseConnectionFactory()
					.CreateConnection(":memory:");

			_deckRepository = new Repository<Deck>(sqliteConnection);
			var flashcardRepository = new Repository<Flashcard>(sqliteConnection);
			var tableCreator = new TableCreator(sqliteConnection);

			ISpacedRepetitionInitializer leitnerInitializer =
				new LeitnerInitializer(
					flashcardRepository,
					_deckRepository,
					tableCreator);
			leitnerInitializer.Initialize();
			return flashcardRepository;
		}

		private readonly ITestOutputHelper _output;

		private Repository<Deck> _deckRepository;
		private readonly ISpacedRepetition _leitner;

		private IEnumerable<Flashcard> Flashcards(string deck)
		{
			return _deckRepository
				.FindMatching(cd => cd.DeckTitle == deck)
				.Result
				.Single()
				.Cards;
		}

		private static QuestionResult Known(Flashcard f)
		{
			return new QuestionResult(f, true);
		}
	}
}