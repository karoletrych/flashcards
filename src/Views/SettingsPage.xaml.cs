using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Infrastructure.Settings;
using Flashcards.Models;
using Flashcards.Services.DataAccess.Database;
using Flashcards.Views.CustomViews;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public class ExportParameters
	{
		public ExportParameters(string databasePath, string exportPath)
		{
			DatabasePath = databasePath;
			ExportPath = exportPath;
		}

		public string DatabasePath { get; }
		public string ExportPath { get; }
	}

	public partial class SettingsPage : ContentPage
	{
		private readonly INotificationScheduler _notificationScheduler;
		private readonly ISetting<TimeSpan> _repetitionTimeSetting;
		private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
		private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
		private readonly ISetting<bool> _shuffleRepetitionsSetting;
		private readonly ISetting<bool> _repetitionDoneTodaySetting;
		private readonly ISetting<int> _sessionNumberSetting;
		private readonly ExportParameters _exportParameters;
		private readonly IMessage _message;
		private readonly IDisconnector _disconnect;

		public SettingsPage (INotificationScheduler notificationScheduler, 
			ISetting<TimeSpan> repetitionTimeSetting,
			ISetting<AskingMode> repetitionAskingModeSetting,
			ISetting<int> maximumFlashcardsInRepetitionSetting,
			ISetting<bool> shuffleRepetitionsSetting,
			ISetting<bool> repetitionDoneTodaySetting,
			ISetting<int> sessionNumberSetting,
			ExportParameters exportParameters,
			IMessage message,
			IDisconnector disconnect)
		{
			InitializeComponent();

			_notificationScheduler = notificationScheduler;
			_repetitionTimeSetting = repetitionTimeSetting;
			_repetitionAskingModeSetting = repetitionAskingModeSetting;
			_maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;
			_shuffleRepetitionsSetting = shuffleRepetitionsSetting;
			_repetitionDoneTodaySetting = repetitionDoneTodaySetting;
			_sessionNumberSetting = sessionNumberSetting;
			_exportParameters = exportParameters;
			_message = message;
			_disconnect = disconnect;

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
			Done.On = _repetitionDoneTodaySetting.Value;
			SessionNumber.Text = _sessionNumberSetting.Value.ToString();

			TimePicker.TimeChanged += UpdateRepetitionTime;
			picker.SelectedIndexChanged += AskingMode_SelectedIndexChanged;
			Shuffle.OnChanged += Shuffle_OnChanged;
			Done.OnChanged += Done_OnChanged;
			MaximumFlashcards.Completed += MaximumFlashcards_OnCompleted;
			SessionNumber.Completed += SessionNumber_OnCompleted;

			var strings = Directory.GetFiles(_exportParameters.ExportPath);
			var importPickerItemsSource = 
				strings
				.Where(p => p.Contains("database"))
				.ToList();
			ImportPicker.ItemsSource = 
				importPickerItemsSource;
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

		private void Done_OnChanged(object sender, ToggledEventArgs e)
		{
			var done = (SwitchCell)sender;
			_repetitionDoneTodaySetting.Value = done.On;
		}

		private void SessionNumber_OnCompleted(object sender, EventArgs e)
		{
			var done = (EntryCell)sender;
			_sessionNumberSetting.Value = int.Parse(done.Text);
		}

		private void ExportButton_OnClicked(object sender, EventArgs e)
		{
			var fileName = Path.GetFileName(_exportParameters.DatabasePath);

			var fileNameWithDateTime =
				Path.GetFileNameWithoutExtension(fileName) + DateTime.Now + Path.GetExtension(fileName);

			var exportFilePath = Path.Combine(_exportParameters.ExportPath, fileNameWithDateTime);

			File.Copy(_exportParameters.DatabasePath, exportFilePath);

			_message.LongAlert($"Exported to: {exportFilePath}");
		}

		private void ImportButton_OnClicked(object sender, EventArgs e)
		{
			if(ImportPicker.SelectedItem == null)
				return;

			_disconnect.Disconnect();

			File.Copy(ImportPicker.SelectedItem.ToString(), _exportParameters.DatabasePath, true);

			_message.ShortAlert("Imported database");
		}
	}
}
