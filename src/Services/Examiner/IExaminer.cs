using System;
using Flashcards.Models;

namespace Flashcards.Services.Examiner
{
	public interface  IExaminer
	{
		void Answer(bool known);
		bool TryAskNextQuestion(out Flashcard question);
		int NumberOfQuestion { get; }

		event EventHandler<QuestionResultsEventArgs> SessionEnded;
	}
}