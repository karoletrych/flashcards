using System;
using Flashcards.Infrastructure.Localization;
using Flashcards.Models;

namespace Flashcards.Domain.ViewModels.Tools
{
	public static class AskingModeLocalizationExtension
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