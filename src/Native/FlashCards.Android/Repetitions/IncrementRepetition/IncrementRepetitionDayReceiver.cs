using Android.Content;
using Autofac;
using Flashcards.Android;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Droid.Repetitions.IncrementRepetition
{
	[BroadcastReceiver(Enabled = true)]
	public class IncrementRepetitionDayReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var container = IocRegistrations.DefaultContainer();
			var repetitionSession = container.Resolve<IRepetitionSession>();
			repetitionSession.Increment();
		}
	}
}