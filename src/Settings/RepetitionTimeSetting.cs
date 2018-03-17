using System;

namespace Flashcards.Settings
{
	class RepetitionTimeSetting : Setting<DateTime>
	{
		protected override string Key => "RepetitionTime";
		protected override DateTime DefaultValue => new DateTime(0, 0, 0, hour: 12, minute: 0, second: 0);
	}
}