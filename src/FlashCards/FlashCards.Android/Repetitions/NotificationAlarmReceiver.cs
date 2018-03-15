using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Widget;
using Autofac;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.Views;

namespace FlashCards.Droid.Repetitions
{
    [BroadcastReceiver(Enabled = true)]
	public class NotificationAlarmReceiver : BroadcastReceiver
    {
	    public override async void OnReceive(Context context, Intent intent)
		{
			try
			{
				IncrementSessionNumber.Increment();

				var spacedRepetition = CreateSpacedRepetition();
				var flashcards = await spacedRepetition.ChooseFlashcards(Settings.RepetitionSessionNumber);

				if (flashcards.Any())
				{
					ShowNotification(context);
				}
			}
			catch (Exception e)
			{
				Toast.MakeText(context, e.ToString(), ToastLength.Long).Show();
			}
		}

	    private static ISpacedRepetition CreateSpacedRepetition()
	    {
		    var containerBuilder = new ContainerBuilder();
		    IocRegistrations.RegisterTypesInIocContainer(containerBuilder);

		    var spacedRepetition =
			    containerBuilder
				    .Build()
				    .Resolve<ISpacedRepetition>();
		    return spacedRepetition;
	    }

	    private static void ShowNotification(Context context)
	    {
		    var intent = new Intent(context, typeof(RepetitionActivity));
		    intent.SetFlags(ActivityFlags.NewTask);
		    var pendingIntent =
			    PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

		    var builder = new Notification.Builder(context)
			    .SetContentTitle("Czas na powtórkę")
			    .SetContentText("Skąd to zwątpienie?")
			    .SetSmallIcon(Resource.Drawable.notification_icon)
			    .SetContentIntent(pendingIntent)
			    .SetAutoCancel(true)
			    .SetVisibility(NotificationVisibility.Public);

#pragma warning disable CS0618 // Type or member is obsolete
			builder.SetDefaults(NotificationDefaults.All);
#pragma warning restore CS0618 // Type or member is obsolete

			var notification = builder.Build();

		    var notificationManager =
			    (NotificationManager)context.GetSystemService(Context.NotificationService);

		    notificationManager.Notify(0, notification);
	    }
    }
}