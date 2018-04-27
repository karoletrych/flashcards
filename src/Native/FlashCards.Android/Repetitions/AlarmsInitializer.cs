using System;
using Android.Content;
using Flashcards.PlatformDependentTools;
using Flashcards.Settings;
using FlashCards.Droid.Repetitions.IncrementRepetition;
using FlashCards.Droid.Repetitions.Notifications;

namespace FlashCards.Droid.Repetitions
{
	public class AlarmsInitializer : IAlarmsInitializer
	{
		private readonly Context _context;
		private readonly ISetting<TimeSpan> _repetitionTimeSetting;

		public AlarmsInitializer(Context context, ISetting<TimeSpan> repetitionTimeSetting)
		{
			_context = context;
			_repetitionTimeSetting = repetitionTimeSetting;
		}

		public void Initialize()
		{
			var alarmScheduler = new AlarmScheduler(_context);

			alarmScheduler.Schedule(_repetitionTimeSetting .Value, typeof(RepetitionNotification));
			alarmScheduler.Schedule(TimeSpan.Zero, typeof(IncrementRepetitionDayReceiver));
		}
	}
}