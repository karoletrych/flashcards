using System;

namespace Flashcards.Services.Examiner
{
	public interface IDisposeNotifying : IDisposable
	{
		event EventHandler Disposed;
	}

	public interface IExaminer : IDisposeNotifying
	{
		void Answer(bool known);
		bool TryAskNextQuestion(out Question question);

		event EventHandler<QuestionResultsEventArgs> SessionEnded;
		int QuestionsCount { get; }
	}
}