using System.Linq;
using Android.App;
using Android.App.Job;
using Android.Content;
using Autofac;
using Flashcards.SpacedRepetition.Provider;

namespace FlashCards.Droid
{
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class RepetitionNotificationService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            var containerBuilder = new ContainerBuilder();
            IocRegistrations.RegisterTypesInIocContainer(containerBuilder);

            var spacedRepetition = 
                containerBuilder
                    .Build()
                    .Resolve<ISpacedRepetition>();
            if (spacedRepetition.ChooseFlashcards().Result.Any())
            {
                ShowNotification();
            }

            return true;
        }

        private void ShowNotification()
        {
            var secondIntent = new Intent(this, typeof(RepetitionActivity));

            var pendingIntent =
                TaskStackBuilder.Create(this)
                    .AddNextIntentWithParentStack(secondIntent)
                    .GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

            var builder = new Notification.Builder(this)
                .SetContentTitle("Czas na powtórkę")
                .SetContentText("Skąd to zwątpienie?")
                .SetSmallIcon(Resource.Drawable.notification_icon)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetVisibility(NotificationVisibility.Public);

            builder.SetDefaults(NotificationDefaults.All);

            var notification = builder.Build();

            var notificationManager =
                GetSystemService(NotificationService) as NotificationManager;

            notificationManager.Notify(0, notification);
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return false;
        }
    }
}