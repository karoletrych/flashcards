using System;
using Flashcards.Localization;
using Flashcards.Models;

namespace Flashcards.ViewModels
{
	public static class AskingModeLocalization
	{
		public static string Localize(this AskingMode askingMode)
		{
			switch (askingMode)
			{
				case AskingMode.Front:
					return AppResources.AskingMode_Front;
				case AskingMode.Back:
					return AppResources.AskingMode_Back;
				case AskingMode.Random:
					return AppResources.AskingMode_Random;
				default:
					throw new ArgumentOutOfRangeException(nameof(askingMode), askingMode, null);
			}
		}
	}
}