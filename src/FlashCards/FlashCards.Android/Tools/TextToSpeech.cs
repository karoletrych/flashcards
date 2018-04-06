using Android.Content;
using Android.Speech.Tts;
using Flashcards.PlatformDependentTools;

namespace FlashCards.Droid.Tools
{
	public class TextToSpeechImplementation : Java.Lang.Object, ITextToSpeech, TextToSpeech.IOnInitListener
	{
		TextToSpeech _speaker;
		string _toSpeak;
		private readonly Context _context;

		public TextToSpeechImplementation(Context context)
		{
			_context = context;
		}

		public void Speak(string text)
		{
			_toSpeak = text;
			if (_speaker == null)
			{
				_speaker = new TextToSpeech(_context, this);
			}
			else
			{
				_speaker.Speak(_toSpeak, QueueMode.Flush, null, null);
			}
		}

		public void OnInit(OperationResult status)
		{
			if (status.Equals(OperationResult.Success))
			{
				_speaker.Speak(_toSpeak, QueueMode.Flush, null, null);
			}
		}
	}
}