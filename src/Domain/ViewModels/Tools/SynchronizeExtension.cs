using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Flashcards.Domain.ViewModels.Tools
{
	public static class SynchronizeExtension
	{
		public static void SynchronizeWith<T>(this Collection<T> collection1, IEnumerable<T> collection2)
		{
			var enumerable = collection2.ToList();
			foreach (var c in enumerable)
				if (!collection1.Contains(c))
					collection1.Add(c);

			var removables = collection1.Where(c => !enumerable.Contains(c)).ToList();
			foreach (var removable in removables)
				collection1.Remove(removable);
		}
	}
}