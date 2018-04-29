using System;

namespace Settings
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
				var hours = AppSettingsWrapper.GetValueOrDefault(_hoursKey, 12);
				_minutesKey = Key + "Minutes";
				var minutes = AppSettingsWrapper.GetValueOrDefault(_minutesKey, 0);
				return new TimeSpan(0, hours, minutes, 0);
			}
			set
			{
				AppSettingsWrapper.AddOrUpdateValue(_hoursKey, value.Hours);
				AppSettingsWrapper.AddOrUpdateValue(_minutesKey, value.Minutes);
			}
		}
	}
}