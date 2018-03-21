using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Services
{
	public interface IRepeatingExaminer
	{
		RepeatingExaminer.NextQuestion TryAskNextQuestion();
		void Answer(bool known);
		IEnumerable<Question> Questions { get; }
		TaskCompletionSource<IEnumerable<QuestionResult>> QuestionResults { get; }
	}
}