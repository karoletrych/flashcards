using System;
using Flashcards.Settings;
using Flashcards.Views.CustomViews;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public partial class SettingsPage : ContentPage
	{
		private readonly INotificationScheduler _notificationScheduler;
		private readonly ISetting<DateTime> _repetitionTimeSetting;

		public SettingsPage (INotificationScheduler notificationScheduler, 
			ISetting<DateTime> repetitionTimeSetting)
		{
			_notificationScheduler = notificationScheduler;
			_repetitionTimeSetting = repetitionTimeSetting;
			InitializeComponent();

			var dateTime = _repetitionTimeSetting.Value;
			TimePicker.Time = new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
			TimePicker.TimeChanged += UpdatePeriod;
		}

		private void UpdatePeriod(object sender, EventArgs e)
		{
			var time = ((TimePickerCell) sender).Time;

			var dateTime = new DateTime(0, 0, 0, time.Hours, time.Minutes, time.Seconds);
			_repetitionTimeSetting.Value = dateTime;
			_notificationScheduler.Schedule(dateTime);
		}
	}
}
