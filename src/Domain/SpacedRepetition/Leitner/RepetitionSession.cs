using System;
using System.Collections.Generic;
using System.Text;
using Flashcards.Infrastructure.Settings;
using Flashcards.SpacedRepetition.Interface;

namespace Flashcards.Domain.SpacedRepetition.Leitner
{
	class RepetitionSession : IRepetitionSession
	{
		private readonly ISetting<bool> _repetitionDoneTodaySetting;
		private readonly ISetting<int> _sessionNumberSetting;
		private readonly ISetting<int> _streakDaysSetting;

		public RepetitionSession(
			ISetting<bool> repetitionDoneTodaySetting,
			ISetting<int> sessionNumberSetting,
			ISetting<int> streakDaysSetting)
		{
			_repetitionDoneTodaySetting = repetitionDoneTodaySetting;
			_sessionNumberSetting = sessionNumberSetting;
			_streakDaysSetting = streakDaysSetting;
		}

		public void Increment()
		{
			if (!_repetitionDoneTodaySetting.Value)
				_streakDaysSetting.Value = 0;

			_repetitionDoneTodaySetting.Value = false;

			var sessionNumber = _sessionNumberSetting.Value;
			
			if (sessionNumber < 9)
				_sessionNumberSetting.Value = sessionNumber + 1;
			else 
				_sessionNumberSetting.Value = 0;
		}

		public int Value => _sessionNumberSetting.Value;
	}
}
