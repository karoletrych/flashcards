using System;

namespace Flashcards.Settings
{
	class RepetitionTimeSetting : Setting<DateTime>
	{
		protected override string Key => "RepetitionTime";
		protected override DateTime DefaultValue => new DateTime();
	}
}