using System;

namespace Flashcards.Infrastructure.PlatformDependentTools
{
	public interface INotificationScheduler
	{
		void Schedule(TimeSpan time);
	}
}