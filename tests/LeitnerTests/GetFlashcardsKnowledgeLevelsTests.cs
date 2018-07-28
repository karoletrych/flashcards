using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flashcards.Domain.SpacedRepetition.Leitner;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;
using Xunit;

namespace LeitnerTests
{
	public class GetFlashcardsKnowledgeLevelsTests
	{
		private readonly IGetFlashcardsKnowledgeLevels _getFlashcardsKnowledgeLevels;

		public GetFlashcardsKnowledgeLevelsTests()
		{
			// TODO: move to common class
			var connection = new Connection(new DatabaseConnectionFactory().CreateInMemoryConnection());
			var repository = new Repository<GetFlashcardsKnowledgeLevels.FlashcardKnowledgeLevelDto>(() => connection);
			var flashcardRepository = new Repository<Flashcard>(() => connection);
			var cardDeckRepository = new Repository<CardDeck>(() => connection);
			var deckRepository = new Repository<Deck>(() => connection);

			ISpacedRepetitionInitializer leitnerInitializer =
				new LeitnerInitializer(
					flashcardRepository,
					deckRepository,
					cardDeckRepository,
					connection);

			leitnerInitializer.InitializeAsync();

			_getFlashcardsKnowledgeLevels = new GetFlashcardsKnowledgeLevels(repository);
		}

		[Fact]
		public async Task LoadsData()
		{
			var lesson = new Lesson
			{
				Id = "1",
			};
			var flashcards = await _getFlashcardsKnowledgeLevels.KnowledgeLevels(lesson);
			var flashcardsList = flashcards.ToList();

		}
	}
}
