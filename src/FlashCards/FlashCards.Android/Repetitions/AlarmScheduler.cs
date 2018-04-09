using System;
using System.Threading;
using Android.App;
using Android.Content;
using Java.Util;

namespace FlashCards.Droid.Repetitions
{
	public class AlarmScheduler
	{
		private readonly Context _context;
		private static int _requestCode = 0;

		public AlarmScheduler(Context context)
		{
			_context = context;
		}

		public void Schedule(TimeSpan time, Type broadcastReceiverType)
		{
			var intent = new Intent(_context, broadcastReceiverType);
			intent.AddFlags(ActivityFlags.NewTask);

			var pendingIntent = PendingIntent.GetBroadcast(_context, _requestCode, intent, PendingIntentFlags.UpdateCurrent);
			Interlocked.Increment(ref _requestCode);

			var alarmManager = (AlarmManager) _context.GetSystemService(Context.AlarmService);
			alarmManager.SetInexactRepeating(
				AlarmType.Rtc,
				AlarmTimeInMillis(time),
				AlarmManager.IntervalDay,
				pendingIntent);
		}

		private static long AlarmTimeInMillis(TimeSpan time)
		{
			var date = Calendar.Instance;
			var currentTime =
				new TimeSpan(0, date.Get(CalendarField.HourOfDay), date.Get(CalendarField.Minute), 0);

			if (time < currentTime)
				date.Add(CalendarField.Date, 1);

			date.Set(CalendarField.HourOfDay, time.Hours);
			date.Set(CalendarField.Minute, time.Minutes);
			date.Set(CalendarField.Second, 0);
			date.Set(CalendarField.Millisecond, 0);
			var dateTimeInMillis = date.TimeInMillis;
			return dateTimeInMillis;
		}
	}
}