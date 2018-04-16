using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Flashcards.ViewModels
{
	internal static class SynchronizeExtension
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

		public static void SynchronizeWith<TSynchronized,TSynchronizing>(
			this Collection<TSynchronized> synchronized, 
			IEnumerable<TSynchronizing> synchronizing, 
			Func<TSynchronizing,TSynchronized> mapper2)
		{
			var list = synchronizing.ToList();
			foreach (var c in list)
				if (!synchronized.Contains(mapper2(c)))
					synchronized.Add(mapper2(c));

			var removables = synchronized.Where(c => !list.Select(mapper2).Contains(c)).ToList();
			foreach (var removable in removables)
				synchronized.Remove(removable);
		}
	}
}