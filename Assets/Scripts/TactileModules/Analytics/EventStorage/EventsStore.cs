using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using UnityEngine;

namespace TactileModules.Analytics.EventStorage
{
	public class EventsStore
	{
		public EventsStore(string directory, int maxPools = 1000)
		{
			this.directory = directory;
			this.maxPools = maxPools;
			this.EnsureEventStoreDirectoryExists();
			this.pools = EventsStore.LoadSortedEventPools(this.GetPersistentPath());
		}

		public static string ConvertToJsonList(string eventsData)
		{
			string str = string.Empty;
			bool flag = EventsStore.HasTrailingComma(eventsData);
			if (flag)
			{
				int startIndex = eventsData.LastIndexOf(",", StringComparison.Ordinal);
				str = eventsData.Remove(startIndex);
			}
			return "[" + str + "]";
		}

		private static bool HasTrailingComma(string eventsData)
		{
			return eventsData[eventsData.Length - 1].Equals(',');
		}

		private static List<EventsPool> LoadSortedEventPools(string path)
		{
			string[] storedPoolFiles = EventsStore.GetStoredPoolFiles(path);
			List<EventsPool> result = EventsStore.LoadEventPools(storedPoolFiles);
			EventsStore.SortPoolsByCreationDateAscending(result);
			EventsStore.RemoveEmptyPools(result);
			return result;
		}

		private static void RemoveEmptyPools(List<EventsPool> pools)
		{
			for (int i = pools.Count - 1; i >= 0; i--)
			{
				EventsPool eventsPool = pools[i];
				if (eventsPool.IsEmpty())
				{
					pools.Remove(eventsPool);
					eventsPool.Delete();
				}
			}
		}

		private static string[] GetStoredPoolFiles(string path)
		{
			return Directory.GetFiles(path, "*.rawevents");
		}

		private static List<EventsPool> LoadEventPools(string[] files)
		{
			List<EventsPool> list = new List<EventsPool>();
			foreach (string filePath in files)
			{
				list.Add(new EventsPool(filePath));
			}
			return list;
		}

		private static void SortPoolsByCreationDateAscending(List<EventsPool> pools)
		{
			pools.Sort(delegate(EventsPool a, EventsPool b)
			{
				DateTime creationTime = a.CreationTime;
				DateTime creationTime2 = b.CreationTime;
				return creationTime.CompareTo(creationTime2);
			});
		}

		public static bool IsStorageException(Exception e)
		{
			return e is IsolatedStorageException || e is UnauthorizedAccessException || e is FileNotFoundException || e is DirectoryNotFoundException;
		}

		public void AddEvent(string eventData)
		{
			this.CheckPools();
			this.CurrentPool.Add(eventData);
		}

		public void Append(string eventsData)
		{
			this.CheckPools();
			this.CurrentPool.Append(eventsData);
		}

		private void CheckPools()
		{
			if (this.CurrentPool == null || this.pools.Count == 0)
			{
				this.CreateAndAddNewPool();
			}
			if (this.CurrentPool.Size > 16000L)
			{
				this.CreateAndAddNewPool();
			}
			if (this.pools.Count > this.maxPools)
			{
				this.RemovePool(this.pools[0]);
			}
		}

		private void CreateAndAddNewPool()
		{
			this.pools.Add(this.CreatePool());
		}

		private EventsPool CreatePool()
		{
			return new EventsPool(this.GetPersistentPath() + DateTime.Now.ToFileTimeUtc().ToString() + ".rawevents");
		}

		public EventsPool LockNextReadableEventsPool()
		{
			EventsPool firstUnlockedPool = this.GetFirstUnlockedPool();
			if (firstUnlockedPool != null)
			{
				firstUnlockedPool.IsLocked = true;
			}
			return firstUnlockedPool;
		}

		public void UnlockPool(EventsPool pool)
		{
			pool.IsLocked = false;
		}

		private EventsPool GetFirstUnlockedPool()
		{
			foreach (EventsPool eventsPool in this.pools)
			{
				if (!eventsPool.IsLocked)
				{
					return eventsPool;
				}
			}
			return null;
		}

		public void RemovePool(EventsPool pool)
		{
			pool.Delete();
			this.pools.Remove(pool);
		}

		private EventsPool CurrentPool
		{
			get
			{
				if (this.pools.Count == 0)
				{
					return null;
				}
				EventsPool eventsPool = this.pools[this.pools.Count - 1];
				if (eventsPool.IsLocked)
				{
					return null;
				}
				return eventsPool;
			}
		}

		public void Clear()
		{
			int num = this.pools.Count - 1;
			for (int i = num; i >= 0; i--)
			{
				this.RemovePool(this.pools[i]);
			}
		}

		private void EnsureEventStoreDirectoryExists()
		{
			Directory.CreateDirectory(this.GetPersistentPath());
		}

		private string GetPersistentPath()
		{
			return Application.persistentDataPath + "/" + this.directory + "/";
		}

		private string directory;

		private const string FILE_EXTENSION = ".rawevents";

		private List<EventsPool> pools = new List<EventsPool>();

		private int maxPools = 1000;
	}
}
