using System.Globalization;

namespace Flashcards.PlatformDependentTools
{
	public interface ILocalize
	{
		void SetLocale(CultureInfo ci);
		CultureInfo GetCurrentCultureInfo();
	}
}