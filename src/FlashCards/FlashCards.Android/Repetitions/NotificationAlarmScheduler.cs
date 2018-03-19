using System;
using Android.App;
using Android.Content;
using Flashcards.Views;
using Java.Util;

namespace FlashCards.Droid.Repetitions
{
	public class NotificationAlarmScheduler : INotificationScheduler
	{
		private readonly Context _context;

		public NotificationAlarmScheduler(Context context)
		{
			_context = context;
		}

		public void Schedule(DateTime time)
		{
			var intent = new Intent(_context, typeof(NotificationAlarmReceiver));
			intent.AddFlags(ActivityFlags.NewTask);

			var pendingIntent = PendingIntent.GetBroadcast(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
			var alarmManager = (AlarmManager) _context.GetSystemService(Context.AlarmService);
			alarmManager.SetInexactRepeating(AlarmType.Rtc,
				AlarmTimeInMillis(time),
				AlarmManager.IntervalDay,
				pendingIntent);
		}

		private static long AlarmTimeInMillis(DateTime time)
		{
			var date = Calendar.Instance;
			var currentTime = new DateTime()
				.AddHours(date.Get(CalendarField.HourOfDay))
				.AddMinutes(date.Get(CalendarField.Minute));

			if (time < currentTime)
				date.Add(CalendarField.Date, 1);

			date.Set(CalendarField.HourOfDay, time.Hour);
			date.Set(CalendarField.Minute, time.Minute);
			date.Set(CalendarField.Second, 0);
			date.Set(CalendarField.Millisecond, 0);
			var dateTimeInMillis = date.TimeInMillis;
			return dateTimeInMillis;
		}
	}
}