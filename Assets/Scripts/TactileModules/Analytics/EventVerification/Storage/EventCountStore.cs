using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Storage.Serialization;
using TactileModules.TactilePrefs;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public class EventCountStore : IEventCountStore
	{
		public EventCountStore(ILocalStorageObject<StoredEvents> storage, ICacheControl cacheControl)
		{
			this.storage = storage;
			this.cacheControl = cacheControl;
			this.Load();
			this.RemoveExpiredEntries();
		}

		private void RemoveExpiredEntries()
		{
			Dictionary<string, int>.KeyCollection keys = this.stored.Events.Keys;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (string key in keys)
			{
				EventStorageKeys eventStorageKeys = EventStorageKeys.FromKey(key);
				if (!this.IsExpired(eventStorageKeys.Date))
				{
					dictionary[key] = this.stored.Events[key];
				}
			}
			this.stored.Events = dictionary;
			this.Save();
		}

		public void Add(EventStorageKeys eventData)
		{
			string text = EventStorageKeys.ToKey(eventData);
			if (!this.stored.Events.ContainsKey(text))
			{
				this.stored.Events.Add(text, 0);
			}
			Dictionary<string, int> events;
			string key;
			(events = this.stored.Events)[key = text] = events[key] + 1;
			this.Save();
		}

		private bool IsExpired(string packagedDate)
		{
			return this.cacheControl.IsExpired(packagedDate);
		}

		public Dictionary<string, int> GetEvents()
		{
			return this.stored.Events;
		}

		public bool IsEmpty()
		{
			return this.stored.Events.Count == 0;
		}

		public void Save()
		{
			this.storage.Save(this.stored);
		}

		public void Load()
		{
			this.stored = this.storage.Load();
		}

		public void Clear()
		{
			this.stored.Events.Clear();
			this.storage.Delete();
		}

		private readonly ILocalStorageObject<StoredEvents> storage;

		private readonly ICacheControl cacheControl;

		private StoredEvents stored;
	}
}
