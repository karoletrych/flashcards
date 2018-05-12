using System;
using System.Reflection;
using System.Resources;
using Flashcards.Infrastructure.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flashcards.Views
{
	// You exclude the 'Extension' suffix when using in XAML
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		private static readonly string ResourceId = typeof(AppResources).FullName;

		static readonly ResourceManager ResourceManager = new ResourceManager(ResourceId, typeof(AppResources).GetTypeInfo().Assembly);

		public string Text { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Text == null)
				return string.Empty;

			var translation = ResourceManager.GetString(Text, AppResources.Culture);
			if (translation == null)
			{
#if DEBUG
				throw new ArgumentException(
					$"Key '{Text}' was not found in resources '{ResourceId}' for culture '{AppResources.Culture.Name}'.",
					"Text");
#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
			}
			return translation;
		}
	}
}