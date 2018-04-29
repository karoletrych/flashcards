using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Settings
{
	public static class AppSettingsWrapper
	{
		public static decimal GetValueOrDefault(string key, decimal defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static bool GetValueOrDefault(string key, bool defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static long GetValueOrDefault(string key, long defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static string GetValueOrDefault(string key, string defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static int GetValueOrDefault(string key, int defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static float GetValueOrDefault(string key, float defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static DateTime GetValueOrDefault(string key, DateTime defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static Guid GetValueOrDefault(string key, Guid defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static double GetValueOrDefault(string key, double defaultValue, string fileName = null)
		{
			return Settings.GetValueOrDefault(key, defaultValue, fileName);
		}

		public static bool AddOrUpdateValue(string key, decimal value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, bool value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, long value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, string value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, int value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, float value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, DateTime value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, Guid value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		public static bool AddOrUpdateValue(string key, double value, string fileName = null)
		{
			return Settings.AddOrUpdateValue(key, value, fileName);
		}

		private static ISettings Settings => CrossSettings.Current;
	}
}