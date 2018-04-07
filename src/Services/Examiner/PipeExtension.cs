using System;

namespace Flashcards.Services.Examiner
{
    static class PipeExtension
    {
	    public static TR Pipe<T, TR>(this T target, Func<T, TR> func)
	    {
		    return func(target);
	    }
	}
}
