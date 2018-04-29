namespace Flashcards.Infrastructure.PlatformDependentTools
{
	public interface IMessage
	{
		void LongAlert(string message);
		void ShortAlert(string message);
	}
}
