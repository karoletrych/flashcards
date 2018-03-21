using System;
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

		public SettingsPage (INotificationScheduler notificationScheduler, 
			ISetting<TimeSpan> repetitionTimeSetting)
		{
			_notificationScheduler = notificationScheduler;
			_repetitionTimeSetting = repetitionTimeSetting;
			InitializeComponent();

			TimePicker.Time = _repetitionTimeSetting.Value;
			TimePicker.TimeChanged += UpdateRepetitionTime;
		}

		private void UpdateRepetitionTime(object sender, EventArgs e)
		{
			var time = ((TimePickerCell) sender).Time;
			
			_repetitionTimeSetting.Value = time;
			_notificationScheduler.Schedule(time);
		}
	}
}
