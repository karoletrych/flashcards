using System;
using Flashcards.PlatformDependentTools;

namespace FlashCards.Droid.Repetitions.IncrementRepetition
{
	public class IncrementRepetitionDayAlarmScheduler : IIncrementRepetitionDaysScheduler
	{
		private readonly AlarmScheduler _alarmScheduler;

		public IncrementRepetitionDayAlarmScheduler(AlarmScheduler alarmScheduler)
		{
			_alarmScheduler = alarmScheduler;
		}

		public void Schedule(TimeSpan time)
		{
			_alarmScheduler.Schedule(time, typeof(IncrementRepetitionDayReceiver));
		}
	}
}