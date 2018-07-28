using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	public class GetFlashcardsKnowledgeLevels : IGetFlashcardsKnowledgeLevels
	{
		private readonly IRepository<FlashcardKnowledgeLevelDto> _deckRepository;

		public GetFlashcardsKnowledgeLevels(IRepository<FlashcardKnowledgeLevelDto> deckRepository)
		{
			_deckRepository = deckRepository;
		}

		public class FlashcardKnowledgeLevelDto
		{
			public string Id { get; set; }
			public string Front { get; set; }
			public string Back { get; set; }
			public DateTime Created { get; set; }
			public string ImageUrl { get; set; }
			public string LessonId { get; set; }

			public string DeckTitle { get; set; }
		}

		public async Task<IEnumerable<FlashcardKnowledgeLevel>> KnowledgeLevels(Lesson lesson)
		{
			var query = @"SELECT f.Front, f.Back, f.Id, f.ImageUrl, f.LessonId, d.DeckTitle 
FROM Flashcard f
JOIN CardDeck cd on cd.CardId = f.Id
JOIN Deck d on cd.DeckId = d.Id
WHERE f.LessonId = ?";
			var data = await _deckRepository.GetUsingSQL(query, lesson.Id);
			return data.Select(d=> new FlashcardKnowledgeLevel(new Flashcard
				{
					Id = d.Id,
					Front = d.Front,
					Back = d.Back,
					ImageUrl = d.ImageUrl,
					LessonId = d.LessonId,
					Created = d.Created
				}, KnowledgeLevel(d.DeckTitle)));
				
		}

		private KnowledgeLevel KnowledgeLevel(string deckTitle)
		{
			switch (deckTitle)
			{
				case DeckTitles.CurrentDeckTitle:
					return Flashcards.SpacedRepetition.Interface.KnowledgeLevel.None;
				case DeckTitles.RetiredDeckTitle:
					return Flashcards.SpacedRepetition.Interface.KnowledgeLevel.Known;
				case string s when !string.IsNullOrEmpty(s):
					return Flashcards.SpacedRepetition.Interface.KnowledgeLevel.Medium;
				default:
					throw new InvalidOperationException(deckTitle);
			}
		}
	}
}