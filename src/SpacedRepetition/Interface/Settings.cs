using Flashcards.Models;
using Flashcards.Settings;

namespace Flashcards.SpacedRepetition.Interface
{
	internal class RepetitionAskingModeSetting : Setting<AskingMode>
	{
		public override AskingMode Value
		{
			get => (AskingMode) AppSettings.GetValueOrDefault(Key, 0);
			set => AppSettings.AddOrUpdateValue(Key, (int) value);
		}

		protected override string Key => "RepetitionAskingMode";
		protected override AskingMode DefaultValue => AskingMode.Front;
	}

	internal class MaximumFlashcardsInRepetitionSetting : Setting<int>
	{
		protected override string Key => "MaximumNumberOfFlashcardsInRepetition";
		protected override int DefaultValue => 20;
	}
}