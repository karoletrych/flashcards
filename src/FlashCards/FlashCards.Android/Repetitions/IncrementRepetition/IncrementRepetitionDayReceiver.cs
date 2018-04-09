﻿using Android.Content;
using Autofac;
using Flashcards.SpacedRepetition.Interface;

namespace FlashCards.Droid.Repetitions.IncrementRepetition
{
	[BroadcastReceiver(Enabled = true)]
	public class IncrementRepetitionDayReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var container = IocRegistrations.DefaultContainer();
			var spacedRepetition = container.ResolveNamed<ISpacedRepetition>("repetitionDoneTodaySetting");
			spacedRepetition.Proceed();
		}
	}
}