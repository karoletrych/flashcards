using Flashcards.Infrastructure.Settings;
using Flashcards.Models;

namespace Flashcards.SpacedRepetition.Interface
{
	internal class RepetitionAskingModeSetting : Setting<AskingMode>
	{
		public override AskingMode Value
		{
			get => (AskingMode) AppSettingsWrapper.GetValueOrDefault(Key, 0);
			set => AppSettingsWrapper.AddOrUpdateValue(Key, (int) value);
		}

		protected override string Key => "RepetitionAskingMode";
		protected override AskingMode DefaultValue => AskingMode.Front;
	}

	internal class MaximumFlashcardsInRepetitionSetting : Setting<int>
	{
		protected override string Key => "MaximumNumberOfFlashcardsInRepetition";
		protected override int DefaultValue => 20;
	}

	internal class ShuffleRepetitionsSetting : Setting<bool>
	{
		protected override string Key => "ShuffleRepetitions";
		protected override bool DefaultValue => true;
	}
}