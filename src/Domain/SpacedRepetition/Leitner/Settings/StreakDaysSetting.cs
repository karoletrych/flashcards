using Flashcards.Infrastructure.Settings;

namespace Flashcards.Domain.SpacedRepetition.Leitner.Settings
{
	class StreakDaysSetting : Setting<int>
	{
		protected override string Key => "StreakDaysSetting";

		protected override int DefaultValue => 0;
	}
}


				