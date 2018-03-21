using System;
using System.Globalization;
using Flashcards.Models;
using Xamarin.Forms;

namespace Flashcards.Views.Converters
{
    class LanguageToFlagConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    var language = (Language)value;
		    var acronym = language.Acronym();
		    var path = "flag_" + acronym + ".png";
		    return path;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new InvalidOperationException();
	    }
    }
}
