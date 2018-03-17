using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Flashcards.Settings
{
	public interface ISetting<T>
	{
		T Value { get; set; }
	}

	public abstract class Setting<T> : ISetting<T>
	{
		protected static ISettings AppSettings => CrossSettings.Current;
		protected abstract string Key { get; }
		protected abstract T DefaultValue { get; }

		public virtual T Value
		{
			get
			{
				switch (DefaultValue)
				{
					case DateTime defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case Guid defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case bool defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case decimal defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case double defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case float defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case int defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case long defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					case string defaultValue:
						return (T) (object) AppSettings.GetValueOrDefault(Key, defaultValue);
					default:
						throw new ArgumentException("Invalid type T: " + typeof(T));
				}
			}
			set
			{
				switch (value)
				{
					case DateTime v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case Guid v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case bool v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case decimal v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case double v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case float v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case int v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case long v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					case string v:
						AppSettings.AddOrUpdateValue(Key, v);
						break;
					default:
						throw new ArgumentException("Invalid type T: " + typeof(T));
				}
			}
		}
	}
}