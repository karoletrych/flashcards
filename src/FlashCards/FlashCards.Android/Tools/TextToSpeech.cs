using System;
using System.Globalization;
using Android.Content;
using Android.Speech.Tts;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Java.Util;

namespace FlashCards.Droid.Tools
{
	public class TextToSpeech : Java.Lang.Object, ITextToSpeech, Android.Speech.Tts.TextToSpeech.IOnInitListener
	{
		readonly Android.Speech.Tts.TextToSpeech _speaker;

		public TextToSpeech(Context context)
		{
			_speaker = new Android.Speech.Tts.TextToSpeech(context, this);
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