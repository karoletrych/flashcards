namespace Flashcards.Settings
{
	public interface ISetting<T>
	{
		T Value { get; set; }
	}
}