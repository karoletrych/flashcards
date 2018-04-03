using Xamarin.Forms;

namespace Flashcards.Views.CustomViews
{
	internal class PickerCell : ViewCell
	{

		private Label _label { get; set; }
		private View _picker { get; set; }
		private StackLayout _layout { get; set; }

		public string Label
		{
			get => _label.Text;
			set => _label.Text = value;
		}

		public View Picker
		{
			set
			{
				if (_picker != null)
				{
					_layout.Children.Remove(_picker);
				}

				_picker = value;
				_layout.Children.Add(_picker);

			}
		}

		internal PickerCell()
		{

			_label = new Label()
			{
				VerticalOptions = LayoutOptions.Center,
				FontSize = 15
			};
			_layout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(15, 5),
				Children =
				{
					_label
				}
			};
			View = _layout;
		}

	}
}