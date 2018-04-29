using Settings;

namespace Flashcards.Domain.SpacedRepetition.Leitner.Settings
{
	class RepetitionDoneTodaySetting : Setting<bool>
	{
		protected override string Key => "RepetitionDoneToday";

		protected override bool DefaultValue => false;
	}
}


				