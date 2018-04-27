using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExtensions
{
    public static class ShuffleExtension
    {
	    private static readonly Random Rng = new Random();

	    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
	    {
		    var list = enumerable.ToList();
		    var n = list.Count;
		    while (n > 1)
		    {
			    n--;
			    var k = Rng.Next(n + 1);
			    var value = list[k];
			    list[k] = list[n];
			    list[n] = value;
		    }

		    return list;
	    }
	}
}
