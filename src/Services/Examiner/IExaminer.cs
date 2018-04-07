using System;

namespace Flashcards.Services.Examiner
{
	public interface IExaminer
	{
		void Answer(bool known);
		bool TryAskNextQuestion(out Question question);

		event EventHandler<QuestionResultsEventArgs> SessionEnded;
		int QuestionsCount { get; }
	}
}