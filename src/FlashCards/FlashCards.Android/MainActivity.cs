using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Flashcards.Views;
using FlashCards.Droid;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Debug = System.Diagnostics.Debug;
using Resource = FlashCards.Droid.Resource;

namespace Flashcards.Droid
{
	[Activity(
		Label = "Flashcards",
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
			ScheduleJob();

			Forms.Init(this, bundle);
			LoadApplication(new App(new AndroidPlatformInitializer()));

			ScheduleJob();
		}

		private void ScheduleJob()
		{
			var componentName =
				new ComponentName(ApplicationContext, Class.FromType(typeof(RepetitionNotificationService)));


			var jobInfo =
				new JobInfo.Builder(1, componentName)
					.SetPeriodic(Properties.RepetitionPeriod * 60 * 1000)
					.Build();

			var jobScheduler = (JobScheduler) ApplicationContext.GetSystemService(JobSchedulerService);
			jobScheduler.Schedule(jobInfo);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Debug.WriteLine(e.ToString());
		}
	}
}