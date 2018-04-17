﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Interface
{
	public interface ISpacedRepetition
    {
        Task<IEnumerable<Flashcard>> CurrentRepetitionFlashcards();
	    IEnumerable<Flashcard> LearnedFlashcards { get; }

        Task SubmitRepetitionResults(IEnumerable<QuestionResult> questionResults);
    }
}
