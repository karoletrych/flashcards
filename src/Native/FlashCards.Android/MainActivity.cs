using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FFImageLoading.Forms.Droid;
using Flashcards.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Debug = System.Diagnostics.Debug;
using Resource = Flashcards.Android.Resource;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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

			AppCenter.Start("abf1250a-0574-42a6-a107-80c261a58d6b", typeof(Analytics), typeof(Crashes));

			CachedImageRenderer.Init(enableFastRenderer: true);


			Forms.Init(this, bundle);
			LoadApplication(new App(new AndroidPlatformInitializer(this)));
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Crashes.TrackError(e.ExceptionObject as Exception);
			Debug.WriteLine(e.ToString());
		}
	}
}