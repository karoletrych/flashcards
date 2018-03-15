namespace Settings
{
	public class IncrementSessionNumber
	{
		private readonly ISetting<int> _leitnerSessionNumberSetting;

		public IncrementSessionNumber(ISetting<int> leitnerSessionNumberSetting)
		{
			_leitnerSessionNumberSetting = leitnerSessionNumberSetting;
		}

		public void Increment()
		{
			var sessionNumber = _leitnerSessionNumberSetting.Value;
			if (sessionNumber < 9)
				_leitnerSessionNumberSetting.Value = sessionNumber + 1;
			else
			{
				_leitnerSessionNumberSetting.Value = 0;
			}
		}
	}
}
