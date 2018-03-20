using Flashcards.Models;
using Flashcards.Settings;

namespace Flashcards.SpacedRepetition.Interface
{
    class RepetitionAskingModeSetting : Setting<AskingMode>
    {
	    protected override string Key => "AskingMode";
	    protected override AskingMode DefaultValue => AskingMode.Front;
    }

	class MaximumFlashcardsInRepetitionSetting : Setting<int>
	{
		protected override string Key => "MaximumNumberOfFlashcardsInRepetition";
		protected override int DefaultValue => 20;
	}
}
