using System;

namespace Flashcards.PlatformDependentTools
{
	public interface INotificationScheduler
	{
		void Schedule(TimeSpan time);
	}
}