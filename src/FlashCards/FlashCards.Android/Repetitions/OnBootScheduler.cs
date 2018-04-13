using Android.App;
using Android.Content;
using Autofac;
using Flashcards.PlatformDependentTools;

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
			IocRegistrations.DefaultContainer().Resolve<IAlarmsInitializer>().Initialize();
		}
	}
}