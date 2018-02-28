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
        public LeitnerTests(ITestOutputHelper output)
        {
            _output = output;
            var databaseConnectionFactory =
                new DatabaseConnectionFactory(new ITableCreator[]
                {
                    new CoreTableCreator(),
                    new LeitnerCardDeckCreator(),
                });
            var sqliteConnection = databaseConnectionFactory.CreateConnection(":memory:");
            _flashcardRepository = new Repository<Flashcard>(sqliteConnection);
            _cardDeckRepository = new Repository<CardDeck>(sqliteConnection);

            _leitner = new LeitnerRepetition(
                _cardDeckRepository,
                _flashcardRepository,
                new PropertiesMock());
            Enumerable
                .Range(1, 100)
                .ToList()
                .ForEach(cardId =>
                    {
                        _flashcardRepository.Insert(
                            new Flashcard
                            {
                                Id = cardId,
                            });
                        _cardDeckRepository.Insert(new CardDeck
                        {
                            CardId = cardId,
                            DeckTitle = DeckTitleEnum.CurrentDeck
                        });
                    }
                );
        }

        private readonly ITestOutputHelper _output;

        private readonly Repository<Flashcard> _flashcardRepository;
        private readonly Repository<CardDeck> _cardDeckRepository;
        private readonly ISpacedRepetition _leitner;

        private IEnumerable<CardDeck> Flashcards(DeckTitleEnum deck)
        {
            return _cardDeckRepository.FindMatching(cd => cd.DeckTitle == deck).Result;
        }

        [Fact]
        public void After10CorrectSessions_AllCardsAreInRetiredDeck()
        {
            for (var i = 0; i < 10; ++i)
            {
                var flashcards = _leitner.ChooseFlashcards().Result;
                _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));

                _output.WriteLine($"session: {i}");
                foreach (var deck in typeof(DeckTitleEnum).GetEnumValues())
                    _output.WriteLine(deck + ": " + Flashcards((DeckTitleEnum) deck).Count());
                _output.WriteLine("");
            }

            Assert.Equal(100, Flashcards(DeckTitleEnum.RetiredDeck).Count());
        }

        [Fact]
        public void AnsweringCorrectlyAllFlashcards_DecreasesNumberOfFlashcardsInTheNextSession()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));
            var rearrangedFlashcards = _leitner.ChooseFlashcards().Result;

            Assert.True(rearrangedFlashcards.Count() < 100);
        }

        [Fact]
        public void AnsweringCorrectlyAllFlashcards_MovesThemToDeckBeginningWithSessionNumber()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            _leitner.RearrangeFlashcards(flashcards.Select(f => (f, true)));

            var session0DeckCards =
                _cardDeckRepository.FindMatching(cd => cd.DeckTitle == DeckTitleEnum._0259).Result;
            Assert.NotEmpty(session0DeckCards);

            var currentDeckCards =
                _cardDeckRepository.FindMatching(cd => cd.DeckTitle == DeckTitleEnum.CurrentDeck).Result;
            Assert.Empty(currentDeckCards);
        }

        [Fact]
        public void ChooseFlashcards_ReturnsAllFromCurrentDeck()
        {
            var flashcards = _leitner.ChooseFlashcards().Result;
            Assert.Equal(100, flashcards.Count());
        }
    }
}