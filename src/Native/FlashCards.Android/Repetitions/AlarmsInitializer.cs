using System;
using Android.Content;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Infrastructure.Settings;
using Flashcards.Droid.Repetitions.IncrementRepetition;
using Flashcards.Droid.Repetitions.Notifications;

namespace Flashcards.Droid.Repetitions
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