using Xamarin.Forms;

namespace Flashcards.Views
{
	public static class RepetitionProperties
	{
		private const string SessionNumberKey = "LeitnerSessionNumber";

		public static int SessionNumber
		{
			get
			{
				if (Application.Current.Properties.TryGetValue(SessionNumberKey, out var sessionNumber))
					return (int) sessionNumber;
				Application.Current.Properties[SessionNumberKey] = 0;
				return 0;
			}
		}

		public static void IncrementSessionNumber()
		{
			var sessionNumber = SessionNumber;
			if (sessionNumber < 9)
				Application.Current.Properties[SessionNumberKey] =
					(int)Application.Current.Properties[SessionNumberKey] + 1;
			else
				Application.Current.Properties[SessionNumberKey] = 0;
		}
	}
}