using System.Threading.Tasks;
using static Xamarin.Forms.Application;

namespace Flashcards.Views
{
	public static class Properties
	{
		private const string SessionNumberKey = "LeitnerSessionNumber";
		private const string NotificationPeriodKey = "NotificationPeriodKey";
		private const int DefaultRepetitionPeriod = 20;

		public static int RepetitionSessionNumber
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
			var sessionNumber = RepetitionSessionNumber;
			if (sessionNumber < 9)
				Current.Properties[SessionNumberKey] =
					(int)Current.Properties[SessionNumberKey] + 1;
			else
			{
				Current.Properties[SessionNumberKey] = 0;
			}

			await Current.SavePropertiesAsync().ConfigureAwait(false);
		}

		public static int RepetitionPeriod
		{
			get
			{
				if (Current == null) // happens when called from MainActivity
				{
					return DefaultRepetitionPeriod;
				}
				if (!Current.Properties.TryGetValue(NotificationPeriodKey, out var period))
				{
					Current.Properties[NotificationPeriodKey] = DefaultRepetitionPeriod;
					return DefaultRepetitionPeriod;
				}
				return (int)period;
			}
			set => Current.Properties[NotificationPeriodKey] = value;
		}
	}
}