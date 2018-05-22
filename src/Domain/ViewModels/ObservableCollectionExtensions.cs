using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Flashcards.Domain.ViewModels
{
    static class ObservableCollectionExtensions
    {
	    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison, bool reverse)
	    {
		    var sortableList = new List<T>(collection);
		    sortableList.Sort(comparison);
			if(reverse)
				sortableList.Reverse();

		    for (var i = 0; i < sortableList.Count; i++)
		    {
			    collection.Move(collection.IndexOf(sortableList[i]), i);
		    }
	    }
	}
}
