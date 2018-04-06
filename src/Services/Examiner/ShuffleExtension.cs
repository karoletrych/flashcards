using System;
using System.Collections.Generic;

namespace Flashcards.Services.Examiner
{
    static class ShuffleExtension
    {
	    private static readonly Random Rng = new Random();

	    public static void Shuffle<T>(this IList<T> list)
	    {
		    var n = list.Count;
		    while (n > 1)
		    {
			    n--;
			    var k = Rng.Next(n + 1);
			    var value = list[k];
			    list[k] = list[n];
			    list[n] = value;
		    }
	    }
	}
}
