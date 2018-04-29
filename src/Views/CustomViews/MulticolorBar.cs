using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Flashcards.Domain.ViewModels.Tools;
using Xamarin.Forms;

namespace Flashcards.Views.CustomViews
{
	[ContentProperty("ItemsSource")]
	public sealed class MulticolorBar : View
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				propertyName: nameof(ItemsSource),
				returnType: typeof(ObservableCollection<MulticolorbarItem>),
				declaringType: typeof(MulticolorBar),
				defaultValue: new ObservableCollection<MulticolorbarItem>());

		public MulticolorBar()
		{
			PropertyChanged += ItemsSourcePropertyChanged;
		}

		private void ItemsSourcePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == nameof(ItemsSource))
			{
				if (ItemsSource is INotifyCollectionChanged itemsSource)
					itemsSource.CollectionChanged += (s, args) => ColorbarItemsChanged?.Invoke(s, args);
			}
		}

		public event EventHandler ColorbarItemsChanged;

		public ObservableCollection<MulticolorbarItem> ItemsSource
		{
			get => (ObservableCollection<MulticolorbarItem>) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		
	}
}