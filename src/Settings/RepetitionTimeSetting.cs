using System;

namespace Flashcards.Settings
{
	class RepetitionTimeSetting : Setting<TimeSpan>
	{
		private string _hoursKey;
		private string _minutesKey;
		protected override string Key => "RepetitionTime";
		protected override TimeSpan DefaultValue => new TimeSpan();

		public override TimeSpan Value
		{
			get
			{
				_hoursKey = Key + "Hours";
				var hours = AppSettings.GetValueOrDefault(_hoursKey, 12);
				_minutesKey = Key + "Minutes";
				var minutes = AppSettings.GetValueOrDefault(_minutesKey, 0);
				return new TimeSpan(0, hours, minutes, 0);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_hoursKey, value.Hours);
				AppSettings.AddOrUpdateValue(_minutesKey, value.Minutes);
			}
		}
	}
}