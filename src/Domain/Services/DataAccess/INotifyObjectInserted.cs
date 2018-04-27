using System;

namespace Flashcards.Services.DataAccess
{
	public interface INotifyObjectInserted<T>
	{
		event EventHandler<T> ObjectInserted;
	}
}