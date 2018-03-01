using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using FlashCards.Droid;
using App = Flashcards.Views.App;

namespace Flashcards.Droid
{
    [Activity(
        Label = "Flashcards",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ScheduleJob();

            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidPlatformInitializer()));
        }

        private void ScheduleJob()
        {
            var componentName = new ComponentName(ApplicationContext, Java.Lang.Class.FromType(typeof(RepetitionNotificationService)));
            var jobInfo = 
                new JobInfo.Builder(1, componentName)
                .SetMinimumLatency(10_000) 
//                .SetPeriodic(1200_000) // 20 minutes
//                .SetPeriodic(21600_000) // 6 hours
                .Build();

            var jobScheduler = (JobScheduler)ApplicationContext.GetSystemService(JobSchedulerService);
            jobScheduler.Schedule(jobInfo);
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ToString());
        }
    }
}

