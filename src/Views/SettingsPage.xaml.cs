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

		public SettingsPage (INotificationScheduler notificationScheduler, 
			ISetting<TimeSpan> repetitionTimeSetting,
			ISetting<AskingMode> repetitionAskingModeSetting,
			ISetting<int> maximumFlashcardsInRepetitionSetting)
		{
			InitializeComponent();

			_notificationScheduler = notificationScheduler;
			_repetitionTimeSetting = repetitionTimeSetting;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
			_maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;

			Initialize();
		}

		private void Initialize()
		{
			TimePicker.Time = _repetitionTimeSetting.Value;
			TimePicker.TimeChanged += UpdateRepetitionTime;

			var picker = new Picker
			{
				ItemsSource = Enum.GetNames(typeof(AskingMode)),
				SelectedIndex = (int) _repetitionAskingModeSetting.Value
			};
			picker.SelectedIndexChanged += AskingMode_SelectedIndexChanged;
			AskingModePickerCell.Picker = picker;
			MaximumFlashcards.Text = _maximumFlashcardsInRepetitionSetting.Value.ToString();
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
	}
}
