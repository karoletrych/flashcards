using System;
using Flashcards.PlatformDependentTools;

namespace FlashCards.Droid.Repetitions.Notifications
{
	public class NotificationScheduler : INotificationScheduler
	{
		private readonly AlarmScheduler _alarmScheduler;

		public NotificationScheduler(AlarmScheduler alarmScheduler)
		{
			_alarmScheduler = alarmScheduler;
		}

		public void Schedule(TimeSpan time)
		{
			_alarmScheduler.Schedule(time, typeof(RepetitionNotification));
		}
	}
}