using System.Collections.Generic;

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	static class DeckTitles
	{
		public const string CurrentDeckTitle = "CurrentDeck";
		public const string RetiredDeckTitle = "RetiredDeck";

		public static readonly ISet<string> Titles = new HashSet<string>
		{
			CurrentDeckTitle,
			RetiredDeckTitle,
			"0259",
			"1360",
			"2471",
			"3582",
			"4693",
			"5704",
			"6815",
			"7926",
			"8037",
			"9148"
		};
	}
}
		