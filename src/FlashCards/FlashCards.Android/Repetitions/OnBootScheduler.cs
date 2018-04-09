using System;
using Android.App;
using Android.Content;
using Autofac;
using Flashcards.Settings;
using FlashCards.Droid.Repetitions.IncrementRepetition;
using FlashCards.Droid.Repetitions.Notifications;

namespace FlashCards.Droid.Repetitions
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[]
	{
		Intent.ActionBootCompleted
	})]
	public class OnBootScheduler : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var alarmScheduler = new AlarmScheduler(context);

			var repetitionTime = 
				IocRegistrations.DefaultContainer().ResolveNamed<ISetting<TimeSpan>>("RepetitionTimeSetting");

			alarmScheduler.Schedule(repetitionTime.Value, typeof(RepetitionNotification));
			alarmScheduler.Schedule(TimeSpan.Zero, typeof(IncrementRepetitionDayReceiver));
		}
	}
}