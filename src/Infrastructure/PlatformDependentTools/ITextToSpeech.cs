using System.Globalization;

namespace Flashcards.PlatformDependentTools
{
	public interface ITextToSpeech
	{
		void Speak(string text, CultureInfo cultureInfo);
	}
}