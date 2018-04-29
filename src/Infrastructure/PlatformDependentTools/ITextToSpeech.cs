using System.Globalization;

namespace Flashcards.Infrastructure.PlatformDependentTools
{
	public interface ITextToSpeech
	{
		void Speak(string text, CultureInfo cultureInfo);
	}
}