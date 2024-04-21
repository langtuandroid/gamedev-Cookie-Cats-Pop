using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Storage;

namespace TactileModules.Analytics.EventVerification.Packaging
{
	public class PackageCreator : IPackageCreator
	{
		public PackageCreator(IPackageMetaData metaData)
		{
			this.metaData = metaData;
		}

		public List<EventCountPackage> Create(Dictionary<string, int> rawCounts)
		{
			return this.PackageEventCountsByDateAndVersion(rawCounts);
		}

		private List<EventCountPackage> PackageEventCountsByDateAndVersion(Dictionary<string, int> rawCounts)
		{
			List<EventCount> list = this.ConvertDictionaryKeysToEventCountList(rawCounts);
			Dictionary<string, EventCountPackage> dictionary = new Dictionary<string, EventCountPackage>();
			foreach (EventCount eventCount in list)
			{
				string key = eventCount.EventDate + eventCount.VersionName + eventCount.VersionCode;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, this.CreateNewPackage(eventCount));
				}
				dictionary[key].EventCounts.Add(eventCount);
			}
			List<EventCountPackage> list2 = new List<EventCountPackage>();
			foreach (string key2 in dictionary.Keys)
			{
				list2.Add(dictionary[key2]);
			}
			return list2;
		}

		private List<EventCount> ConvertDictionaryKeysToEventCountList(Dictionary<string, int> dictionary)
		{
			List<EventCount> list = new List<EventCount>();
			foreach (string key in dictionary.Keys)
			{
				EventStorageKeys eventStorageKeys = this.ParseKeyToMetaProperties(key);
				EventCount item = new EventCount
				{
					EventSchemaHash = eventStorageKeys.EventSchemaHash,
					EventName = eventStorageKeys.EventName,
					VersionName = eventStorageKeys.VersionName,
					VersionCode = eventStorageKeys.VersionCode,
					EventDate = eventStorageKeys.Date,
					Count = dictionary[key]
				};
				list.Add(item);
			}
			return list;
		}

		private EventCountPackage CreateNewPackage(EventCount eventCount)
		{
			return new EventCountPackage
			{
				UserId = this.metaData.GetUserId(),
				Platform = this.metaData.GetPlatform(),
				EventDate = eventCount.EventDate,
				VersionName = eventCount.VersionName,
				VersionCode = eventCount.VersionCode
			};
		}

		private EventStorageKeys ParseKeyToMetaProperties(string key)
		{
			return EventStorageKeys.FromKey(key);
		}

		private readonly IPackageMetaData metaData;
	}
}
