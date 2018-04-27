namespace Flashcards.SpacedRepetition.Interface
{
	public interface IRepetitionSession
	{
		void Increment();
		int Value { get; }
	}
}