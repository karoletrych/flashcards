using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.DataAccess.Database;
using Flashcards.SpacedRepetition.Provider;
using Xunit;
using Xunit.Abstractions;
using static Flashcards.SpacedRepetition.Leitner.Models;
using static Flashcards.SpacedRepetition.Leitner.Algorithm;

namespace LeitnerTests
{
    internal class PropertiesMock : IProperties
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public object Get(string key)
        {
            return _properties[key];
        }

        public void Set(string key, object value)
        {
            _properties[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }
    }

    public class LeitnerTests
    {
        private const int FlashcardCount = 20;

        public LeitnerTests(ITestOutputHelper output)
        {
            _output = output;
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

            Enumerable
                .Range(1, FlashcardCount)
                .Select(cardId =>
                    new Flashcard
                    {
                        Id = cardId,
                        LessonId = 1
                    })
                .ToList()
                .ForEach(f => flashcardRepository.Insert(f));

            _leitner = new LeitnerRepetition(
                _deckRepository,
                new PropertiesMock());

        }

        private readonly ITestOutputHelper _output;

        private readonly Repository<Deck> _deckRepository;
        private readonly ISpacedRepetition _leitner;

        private IEnumerable<Flashcard> Flashcards(string deck)
        {
            return _deckRepository
                .FindMatching(cd => cd.DeckTitle == deck)
                .Result
                .Single()
                .Cards;
        }

        [Fact]
        public void After10CorrectSessions_AllCardsAreInRetiredDeck()
        {
            for (var i = 0; i < 20; ++i)
            {
                var flashcards = _leitner.ChooseFlashcards().Result.ToList();
                _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));
                _output.WriteLine($"session: {i}");
                foreach (var deck in _deckRepository.FindAll().Result)
                    _output.WriteLine(deck.DeckTitle + ": " + deck.Cards.Count());
                _output.WriteLine("");
            }

            Assert.Equal(FlashcardCount, Flashcards(retiredDeckName).Count());
        }

        [Fact]
        public void AnsweringCorrectlyAllFlashcards_DecreasesNumberOfFlashcardsInTheNextSession()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));
            var rearrangedFlashcards = _leitner.ChooseFlashcards().Result;

            Assert.NotEqual(FlashcardCount, rearrangedFlashcards.Count());
        }

        [Fact]
        public void AnsweringCorrectlyAllFlashcards_MovesThemToDeckBeginningWithSessionNumber()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));

            var session0DeckCards =
                _deckRepository.FindMatching(cd => cd.DeckTitle == "0259").Result;
            Assert.NotEmpty(session0DeckCards);

            var currentDeckCards =
                _deckRepository.FindMatching(cd => cd.DeckTitle == currentDeckName).Result.Single().Cards;
            Assert.Empty(currentDeckCards);
        }

        [Fact]
        public void ChooseFlashcards_ReturnsAllFromCurrentDeck()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            Assert.Equal(FlashcardCount, flashcards.Count());
        }
    }
}