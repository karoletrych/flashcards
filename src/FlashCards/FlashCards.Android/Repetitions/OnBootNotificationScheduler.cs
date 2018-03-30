using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Autofac;
using Flashcards.Settings;
using Flashcards.Views;

namespace FlashCards.Droid.Repetitions
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[]
	{
		Intent.ActionBootCompleted
	})]
	public class OnBootNotificationScheduler : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var notificationScheduler = new NotificationAlarmScheduler(context);
			var repetitionTime = 
				IocRegistrations.DefaultContainer().ResolveNamed<ISetting<TimeSpan>>("RepetitionTimeSetting");
			notificationScheduler.Schedule(repetitionTime.Value);
		}
	}
}