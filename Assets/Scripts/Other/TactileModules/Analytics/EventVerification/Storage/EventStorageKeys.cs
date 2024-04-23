using System;
using System.Text;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public class EventStorageKeys
	{
		public string EventSchemaHash { get; set; }

		public string EventName { get; set; }

		public int VersionCode { get; set; }

		public string VersionName { get; set; }

		public string Date { get; set; }

		public static string ToKey(EventStorageKeys storageKeys)
		{
			return new StringBuilder().Append(storageKeys.EventSchemaHash).Append("|").Append(storageKeys.EventName).Append("|").Append(storageKeys.VersionCode).Append("|").Append(storageKeys.VersionName).Append("|").Append(storageKeys.Date).ToString();
		}

		public static EventStorageKeys FromKey(string key)
		{
			string[] array = key.Split(new char[]
			{
				'|'
			});
			return new EventStorageKeys
			{
				EventSchemaHash = array[0],
				EventName = array[1],
				VersionCode = int.Parse(array[2]),
				VersionName = array[3],
				Date = array[4]
			};
		}

		public bool Equals(EventStorageKeys other)
		{
			return this.EventName.Equals(other.EventName) && this.EventSchemaHash.Equals(other.EventSchemaHash) && this.VersionCode.Equals(other.VersionCode) && this.VersionName.Equals(other.VersionName) && this.Date.Equals(other.Date);
		}
	}
}
