using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Flashcards.Views.CustomViews
{
	class ImageGrid : Grid
	{
		public static readonly BindableProperty SourceProperty = BindableProperty.Create(
			nameof(Source),
			typeof(IList<Uri>),
			typeof(ImageGrid),
			new List<Uri>());

		public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
			nameof(Selected),
			typeof(Uri),
			typeof(ImageGrid),
			null);

		public int NumberOfColumns { get; set; }
		public int NumberOfRows { get; set; }

		public IList<Uri> Source
		{
			get => (IList<Uri>) GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public Uri Selected
		{
			get => (Uri) GetValue(SelectedProperty);
			set => SetValue(SelectedProperty, value);
		}

		public Color SelectionFrameColor { get; set; }

		protected override void OnBindingContextChanged()
		{
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			if (propertyName == nameof(Source))
			{
				for (var i = 0; i < NumberOfColumns; ++i)
					ColumnDefinitions.Add(
						new ColumnDefinition
						{
							Width = GridLength.Star
						});
				for (var j = 0; j < NumberOfRows; ++j)
					RowDefinitions.Add(
						new RowDefinition
						{
							Height = GridLength.Star
						});

				for (var row = 0; row < NumberOfRows; ++row)
				for (var column = 0; column < NumberOfColumns; ++column)
				{
					var index = row * NumberOfColumns + column;
					var image = new Image
					{
						Source = Source[index],
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						WidthRequest = 300,
						HeightRequest = 300
					};
					var framedImage = new Frame
					{
						Content = image,
						IsVisible = true,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						WidthRequest = 300,
						HeightRequest = 300
					};
					var tapGestureRecognizer = new TapGestureRecognizer();
					tapGestureRecognizer.Tapped += (view, e) => ((Frame) view).OutlineColor = SelectionFrameColor;
					framedImage.GestureRecognizers.Add(tapGestureRecognizer);
					Children.Add(framedImage, column, row);
				}
			}
		}
	}
}