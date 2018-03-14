using System;
using Flashcards.Views.CustomViews;
using Xamarin.Forms;

namespace Flashcards.Views
{
	public partial class SettingsPage : ContentPage
	{
		private readonly INotificationScheduler _notificationScheduler;

		public SettingsPage (INotificationScheduler notificationScheduler)
		{
			_notificationScheduler = notificationScheduler;
			InitializeComponent();

			TimePicker.Time = Settings.RepetitionTime;
			TimePicker.TimeChanged += UpdatePeriod;
		}

		private void UpdatePeriod(object sender, EventArgs e)
		{
			var time = ((TimePickerCell) sender).Time;

			Settings.RepetitionTime = time;
			_notificationScheduler.Schedule(time);
		}
	}
}
