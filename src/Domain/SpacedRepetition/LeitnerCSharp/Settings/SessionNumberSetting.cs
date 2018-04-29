using Flashcards.Infrastructure.Settings;

namespace Flashcards.Domain.SpacedRepetition.Leitner.Settings
{
	class SessionNumberSetting : Setting<int>
	{
		protected override string Key => "SessionNumber";
		protected override int DefaultValue => 0;
	}
}


				