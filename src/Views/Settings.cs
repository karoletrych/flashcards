
// Helpers/Settings.cs

using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Flashcards.Views
{
	public static class Settings
	{
		private static ISettings AppSettings => CrossSettings.Current;

		private const string SessionNumberKey = "LeitnerSessionNumber";
		private const string NotificationTimeKey = "NotificationHourKey";

		private static readonly int DefaultSessionNumber = 0;
		private static readonly int DefaultRepetitionHour = 12;

		public static int RepetitionSessionNumber
		{
			get => AppSettings.GetValueOrDefault(SessionNumberKey, DefaultSessionNumber);
			set => AppSettings.AddOrUpdateValue(SessionNumberKey, value);
		}

		public static TimeSpan RepetitionTime
		{
			get => 
				TimeSpan.FromHours(AppSettings.GetValueOrDefault(NotificationTimeKey, DefaultRepetitionHour));
			set => AppSettings.AddOrUpdateValue(NotificationTimeKey, value.Hours);
		}
	}

	public static class IncrementSessionNumber
	{
		// TODO: create interface for "proceeding" repetition
		public static void Increment()
		{
			var sessionNumber = Settings.RepetitionSessionNumber;
			if (sessionNumber < 9)
				Settings.RepetitionSessionNumber = sessionNumber + 1;
			else
			{
				Settings.RepetitionSessionNumber = 0;
			}
		}
	}
}
