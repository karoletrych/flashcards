using System;

namespace Flashcards.Settings
{
	public abstract class Setting<T> : ISetting<T>
	{
		protected abstract string Key { get; }
		protected abstract T DefaultValue { get; }

		public virtual T Value
		{
			get
			{
				switch (DefaultValue)
				{
					case DateTime defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case Guid defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case bool defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case decimal defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case double defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case float defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case int defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case long defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					case string defaultValue:
						return (T) (object) AppSettingsWrapper.GetValueOrDefault(Key, defaultValue);
					default:
						throw new ArgumentException("Invalid type T: " + typeof(T));
				}
			}
			set
			{
				switch (value)
				{
					case DateTime v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case Guid v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case bool v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case decimal v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case double v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case float v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case int v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case long v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					case string v:
						AppSettingsWrapper.AddOrUpdateValue(Key, v);
						break;
					default:
						throw new ArgumentException("Invalid type T: " + typeof(T));
				}
			}
		}
	}
}