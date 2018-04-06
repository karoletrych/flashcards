using System;
using Android.App;
using Android.App.Job;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Flashcards.Views;
using FlashCards.Droid;
using Java.Util;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Debug = System.Diagnostics.Debug;
using Resource = FlashCards.Droid.Resource;

namespace Flashcards.Droid
{
	[Activity(
		Label = "@string/app_name",
		Icon = "@drawable/icon",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			CachedImageRenderer.Init(enableFastRenderer: true);


			Forms.Init(this, bundle);
			LoadApplication(new App(new AndroidPlatformInitializer(this)));
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Debug.WriteLine(e.ToString());
		}
	}
}