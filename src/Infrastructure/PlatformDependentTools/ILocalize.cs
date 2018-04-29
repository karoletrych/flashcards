using System.Globalization;

namespace Flashcards.Infrastructure.PlatformDependentTools
{
	public interface ILocalize
	{
		void SetLocale(CultureInfo ci);
		CultureInfo GetCurrentCultureInfo();
	}
}