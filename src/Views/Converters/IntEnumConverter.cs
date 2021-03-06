﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Flashcards.Views.Converters
{
	class IntEnumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int) value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				return Enum.ToObject(targetType, value);
			}
			return 0;
		}
	}
}
