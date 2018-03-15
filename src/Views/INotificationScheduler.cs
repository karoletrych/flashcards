using System;

namespace Flashcards.Views
{
	public interface INotificationScheduler
	{
		void Schedule(DateTime time);
	}
}