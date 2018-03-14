using Android.App;
using Android.Content;
using Android.Widget;
using Flashcards.Views;

namespace FlashCards.Droid.Repetitions
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[]
	{
		Intent.ActionBootCompleted
	})]
	public class BootReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var notificationScheduler = new NotificationAlarmScheduler(context);
			notificationScheduler.Schedule(Settings.RepetitionTime);
		}
	}
}