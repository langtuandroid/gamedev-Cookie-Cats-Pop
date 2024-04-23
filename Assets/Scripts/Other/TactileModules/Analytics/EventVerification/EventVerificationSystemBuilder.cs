using System;
using TactileModules.Analytics.CollectorLoadBalancing;
using TactileModules.Analytics.EventVerification.Packaging;
using TactileModules.Analytics.EventVerification.Storage;
using TactileModules.Analytics.EventVerification.Storage.Date;
using TactileModules.Analytics.EventVerification.Storage.Serialization;
using TactileModules.Analytics.EventVerification.Uploading;
using TactileModules.TactilePrefs;

namespace TactileModules.Analytics.EventVerification
{
	public static class EventVerificationSystemBuilder
	{
		public static EventCountLogger Build(CollectorLoadBalancer collectorLoadBalancer, ServicePath servicePath, string countStorageKey, string packageStorageKey, string userId)
		{
			string domainNamespace = "ECVS_";
			StorageDate storageDate = new StorageDate();
			CacheControl cacheControl = new CacheControl(storageDate, 30);
			LocalStorageJSONObject<StoredEvents> storage = new LocalStorageJSONObject<StoredEvents>(new PlayerPrefsSignedString(domainNamespace, countStorageKey));
			EventCountStore countStore = new EventCountStore(storage, cacheControl);
			LocalStorageJSONObject<StoredPackages> storage2 = new LocalStorageJSONObject<StoredPackages>(new PlayerPrefsSignedString(domainNamespace, packageStorageKey));
			PackageStore packageStore = new PackageStore(storage2, cacheControl);
			PackageMetaData metaData = new PackageMetaData(userId);
			PackageCreator packager = new PackageCreator(metaData);
			RequestFactory requestFactory = new RequestFactory(collectorLoadBalancer, servicePath);
			PackageUploader uploader = new PackageUploader(requestFactory);
			return new EventCountLogger(countStore, uploader, packager, packageStore, storageDate);
		}
	}
}
