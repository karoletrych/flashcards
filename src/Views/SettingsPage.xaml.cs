using System;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Settings;
using Flashcards.Views.CustomViews;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public partial class SettingsPage : ContentPage
	{
		private readonly INotificationScheduler _notificationScheduler;
		private readonly ISetting<TimeSpan> _repetitionTimeSetting;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly ISetting<bool> _shuffleRepetitionsSetting;

		public SettingsPage (INotificationScheduler notificationScheduler, 
			ISetting<TimeSpan> repetitionTimeSetting,
			ISetting<AskingMode> repetitionAskingModeSetting,
			ISetting<int> maximumFlashcardsInRepetitionSetting,
			ISetting<bool> shuffleRepetitionsSetting)
		{
			InitializeComponent();

			_notificationScheduler = notificationScheduler;
			_repetitionTimeSetting = repetitionTimeSetting;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
			_maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;
			_shuffleRepetitionsSetting = shuffleRepetitionsSetting;

			Initialize();
		}

		private void Initialize()
		{
			TimePicker.Time = _repetitionTimeSetting.Value;

			var picker = new Picker
			{
				ItemsSource = Enum.GetNames(typeof(AskingMode)),
				SelectedIndex = (int) _repetitionAskingModeSetting.Value
			};
			AskingModePickerCell.Picker = picker;
			MaximumFlashcards.Text = _maximumFlashcardsInRepetitionSetting.Value.ToString();
			Shuffle.On = _shuffleRepetitionsSetting.Value;

			TimePicker.TimeChanged += UpdateRepetitionTime;
			picker.SelectedIndexChanged += AskingMode_SelectedIndexChanged;
			Shuffle.OnChanged += Shuffle_OnChanged;
			MaximumFlashcards.Completed += MaximumFlashcards_OnCompleted;
		}

		private void UpdateRepetitionTime(object sender, EventArgs e)
		{
			var time = ((TimePickerCell) sender).Time;
			
			_repetitionTimeSetting.Value = time;
			_notificationScheduler.Schedule(time);
		}

		private void AskingMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedMode = (AskingMode)((Picker) sender).SelectedIndex;

			_repetitionAskingModeSetting.Value = selectedMode;
		}

		private void MaximumFlashcards_OnCompleted(object sender, EventArgs e)
		{
			var entryCell = (EntryCell) sender;
			_maximumFlashcardsInRepetitionSetting.Value = int.Parse(entryCell.Text);
		}

		private void Shuffle_OnChanged(object sender, ToggledEventArgs e)
		{
			var shuffle = (SwitchCell) sender;
			_shuffleRepetitionsSetting.Value = shuffle.On;
		}
	}
}
