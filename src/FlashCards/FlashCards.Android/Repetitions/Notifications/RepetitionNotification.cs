using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Autofac;
using Flashcards.Services;

namespace FlashCards.Droid.Repetitions.Notifications
{
    [BroadcastReceiver(Enabled = true)]
	public class RepetitionNotification : BroadcastReceiver
    {
	    public override async void OnReceive(Context context, Intent intent)
		{
			try
			{
				var container = IocRegistrations.DefaultContainer();
				var repeatingExaminer = await container.Resolve<IRepetitionExaminerBuilder>().Examiner();

				if (repeatingExaminer.QuestionsCount > 0)
				{
					ShowNotification(context);
				}
			}
			catch (Exception e)
			{
				Toast.MakeText(context, e.ToString(), ToastLength.Long).Show();
			}
		}

	    private static void ShowNotification(Context context)
	    {
		    var intent = new Intent(context, typeof(RepetitionActivity));
		    intent.SetFlags(ActivityFlags.NewTask);
		    var pendingIntent =
			    PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

		    var builder = new Notification.Builder(context)
			    .SetContentTitle(context.Resources.GetString(Resource.String.time_for_repetition_primary))
			    .SetContentText(context.Resources.GetString(Resource.String.time_for_repetition_secondary))
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