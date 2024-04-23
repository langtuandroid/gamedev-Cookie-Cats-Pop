using System;
using System.Collections.Generic;
using TactileModules.Analytics.EventVerification.Packaging;
using TactileModules.Analytics.EventVerification.Storage;
using TactileModules.Analytics.EventVerification.Storage.Date;
using TactileModules.Analytics.EventVerification.Uploading;

namespace TactileModules.Analytics.EventVerification
{
	public class EventCountLogger : IEventCountLogger
	{
		public EventCountLogger(IEventCountStore countStore, IPackageUploader uploader, IPackageCreator packager, IPackageStore packageStore, IStorageDate date)
		{
			this.countStore = countStore;
			this.uploader = uploader;
			this.packager = packager;
			this.packageStore = packageStore;
			this.date = date;
			this.SetEventHandlers();
		}

		private void SetEventHandlers()
		{
			this.uploader.PackageUploaded += this.UploaderOnPackageUploaded;
		}

		public void LogEvent(string schemaHash, string eventName, double unixTimestamp)
		{
			this.AddEventOccurrenceToCountStore(schemaHash, eventName, unixTimestamp);
			if (this.uploader.IsUploading())
			{
				return;
			}
			if (!this.packageStore.IsEmpty())
			{
				this.UploadStoredPackages();
				return;
			}
			this.PackageEventsAndStartUpload();
		}

		private void PackageEventsAndStartUpload()
		{
			List<EventCountPackage> packages = this.CreatePackages();
			this.AddPackagesToUploadStore(packages);
			this.ClearCountStore();
			this.UploadStoredPackages();
		}

		private void AddEventOccurrenceToCountStore(string schemaHash, string eventName, double unixTimestamp)
		{
			this.countStore.Add(new EventStorageKeys
			{
				EventSchemaHash = schemaHash,
				EventName = eventName,
				Date = StorageDate.FromUnixTimeStamp((long)unixTimestamp),
				VersionName = SystemInfoHelper.BundleShortVersion,
				VersionCode = int.Parse(SystemInfoHelper.BundleVersion)
			});
		}

		private void UploadStoredPackages()
		{
			List<EventCountPackage> packages = this.packageStore.GetPackages();
			this.uploader.UpLoadPackages(packages);
		}

		private List<EventCountPackage> CreatePackages()
		{
			return this.packager.Create(this.countStore.GetEvents());
		}

		private void AddPackagesToUploadStore(List<EventCountPackage> packages)
		{
			this.packageStore.Add(packages);
		}

		private void ClearCountStore()
		{
			this.countStore.Clear();
		}

		private void UploaderOnPackageUploaded(EventCountPackage package)
		{
			this.packageStore.Remove(package);
			if (this.packageStore.IsEmpty())
			{
				if (this.countStore.IsEmpty())
				{
					return;
				}
				this.PackageEventsAndStartUpload();
			}
		}

		private readonly IEventCountStore countStore;

		private readonly IPackageStore packageStore;

		private readonly IPackageUploader uploader;

		private readonly IPackageCreator packager;

		private readonly IStorageDate date;
	}
}
