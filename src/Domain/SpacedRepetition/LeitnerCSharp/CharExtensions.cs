namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	static class CharExtensions
	{
		public static int ToInt(this char c)
		{
			return (int)System.Char.GetNumericValue(c);
		}

		public static bool IsDigit(this char c, int i)
		{
			return System.Char.IsDigit(c) && ToInt(c) == i;
		}
	}
}