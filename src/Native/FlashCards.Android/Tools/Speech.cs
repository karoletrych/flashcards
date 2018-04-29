using System;
using System.Globalization;
using Android.Content;
using Android.Speech.Tts;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Models;
using Java.Util;
using TextToSpeech = Android.Speech.Tts.TextToSpeech;

namespace Flashcards.Droid.Tools
{
	public class Speech : Java.Lang.Object, ITextToSpeech, TextToSpeech.IOnInitListener
	{
		private readonly TextToSpeech _speaker;

		public Speech(Context context)
		{
			_speaker = new TextToSpeech(context, this);
		}

		private static Locale ToLocale(CultureInfo language)
		{
			switch (language.TwoLetterISOLanguageName)
			{
				case "de":
					return Locale.German;
				case "en":
					return Locale.English;
				case "pl":
					return Locale.ForLanguageTag("pl");
				case "fr":
					return Locale.French;
				case "it":
					return Locale.Italian;
				case "es":
					return Locale.ForLanguageTag("es");
				case "sv":
					return Locale.ForLanguageTag("sv");
				case "no":
					return Locale.ForLanguageTag("no");
				case "ru":
					return Locale.ForLanguageTag("ru");
				default:
					throw new ArgumentException();
			}
		}

		public void Speak(string text, CultureInfo cultureInfo)
		{
			_speaker.SetLanguage(ToLocale(cultureInfo));
			_speaker.Speak(text, QueueMode.Flush, null, null);
		}

		public void OnInit(OperationResult status)
		{
			
		}
	}
}