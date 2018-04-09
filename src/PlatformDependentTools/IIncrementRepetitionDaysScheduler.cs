using System;

namespace Flashcards.PlatformDependentTools
{
	public interface IIncrementRepetitionDaysScheduler
	{
		void Schedule(TimeSpan time);
	}
}