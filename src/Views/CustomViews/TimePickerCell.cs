using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Flashcards.Views.CustomViews
{
	internal class TimePickerCell : ViewCell
	{
		private readonly Label _label;
		private readonly TimePicker _timePicker = new TimePicker();

		internal TimePickerCell()
		{
			_label = new Label
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Start,
				FontSize = 15
			};

			_timePicker.HorizontalOptions = LayoutOptions.End;
			
			View = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(15, 5),
				Children =
				{
					_label,
					_timePicker
				}
			};
			_timePicker.PropertyChanged += InvokeTimeChanged;
		}

		private void InvokeTimeChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(TimePicker.Time))
				TimeChanged?.Invoke(this, e);
		}

		public string Label
		{
			get => _label.Text;
			set => _label.Text = value;
		}

		public TimeSpan Time
		{
			get => _timePicker.Time;
			set => _timePicker.Time = value;
		}

		public event EventHandler TimeChanged;
	}
}