using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Packaging;
using TactileModules.Analytics.EventVerification.Storage.Serialization;
using TactileModules.TactilePrefs;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public class PackageStore : IPackageStore
	{
		public PackageStore(ILocalStorageObject<StoredPackages> storage, ICacheControl cacheControl)
		{
			this.storage = storage;
			this.cacheControl = cacheControl;
			this.Load();
			this.RemoveExpiredEntries();
		}

		public void Add(List<EventCountPackage> packagedEvents)
		{
			this.stored.Packages.AddRange(packagedEvents);
			this.Save();
		}

		private void RemoveExpiredEntries()
		{
			List<EventCountPackage> packages = this.stored.Packages;
			List<EventCountPackage> list = new List<EventCountPackage>();
			foreach (EventCountPackage eventCountPackage in packages)
			{
				if (!this.IsExpired(eventCountPackage))
				{
					list.Add(eventCountPackage);
				}
			}
			this.stored.Packages = list;
			this.Save();
		}

		private bool IsExpired(EventCountPackage package)
		{
			return this.cacheControl.IsExpired(package.EventDate);
		}

		public void Remove(EventCountPackage package)
		{
			this.stored.Packages.Remove(package);
			this.Save();
		}

		public List<EventCountPackage> GetPackages()
		{
			return this.stored.Packages;
		}

		public bool IsEmpty()
		{
			return this.stored.Packages.Count == 0;
		}

		private void Save()
		{
			this.storage.Save(this.stored);
		}

		public void Load()
		{
			this.stored = this.storage.Load();
		}

		private readonly ILocalStorageObject<StoredPackages> storage;

		private readonly ICacheControl cacheControl;

		private StoredPackages stored;
	}
}
