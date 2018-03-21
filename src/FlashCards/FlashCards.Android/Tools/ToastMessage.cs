﻿using Android.App;
using Android.Widget;
using Flashcards.PlatformDependentTools;

namespace FlashCards.Droid.Tools
{
	class ToastMessage
	{
		public class MessageAndroid : IMessage
		{
			public void LongAlert(string message)
			{
				Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
			}

			public void ShortAlert(string message)
			{
				Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
			}
		}
	}
}