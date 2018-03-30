using System;
using System.Reflection;
using System.Resources;
using Flashcards.Localization;
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

			var translation = ResourceManager.GetString(Text, Flashcards.Localization.AppResources.Culture);
			if (translation == null)
			{
#if DEBUG
				throw new ArgumentException(
					string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, AppResources.Culture.Name),
					"Text");
#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
			}
			return translation;
		}
	}
}