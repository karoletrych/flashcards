using System.Threading.Tasks;
using static Xamarin.Forms.Application;

namespace Flashcards.Views
{
	public static class RepetitionProperties
	{
		private const string SessionNumberKey = "LeitnerSessionNumber";

		public static int SessionNumber
		{
			get
			{
				if (Current.Properties.TryGetValue(SessionNumberKey, out var sessionNumber))
					return (int) sessionNumber;
				Current.Properties[SessionNumberKey] = 0;
				return 0;
			}
		}

		public static async Task IncrementSessionNumber()
		{
			var sessionNumber = SessionNumber;
			if (sessionNumber < 9)
				Current.Properties[SessionNumberKey] =
					(int)Current.Properties[SessionNumberKey] + 1;
			else
			{
				Current.Properties[SessionNumberKey] = 0;
			}

			await Current.SavePropertiesAsync().ConfigureAwait(false);
		}
	}
}