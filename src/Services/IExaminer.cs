using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
	public interface IExaminer
	{
		IEnumerable<Question> Questions { get; }
		TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults { get; }
		void Answer(bool known);
		bool TryAskNextQuestion(out Flashcard question);
	}
}