using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Interface
{
	public enum KnowledgeLevel
	{
		None = 1,
		Medium = 2,
		Known = 3
	}

	public class FlashcardKnowledgeLevel
	{
		public FlashcardKnowledgeLevel(Flashcard flashcard, KnowledgeLevel knowledgeLevel)
		{
			Flashcard = flashcard;
			KnowledgeLevel = knowledgeLevel;
		}

		public Flashcard Flashcard { get; }
		public KnowledgeLevel KnowledgeLevel { get; }
	}

    public interface IGetFlashcardsKnowledgeLevels
    {
	    Task<IEnumerable<FlashcardKnowledgeLevel>> KnowledgeLevels(Lesson lesson);
    }
}
