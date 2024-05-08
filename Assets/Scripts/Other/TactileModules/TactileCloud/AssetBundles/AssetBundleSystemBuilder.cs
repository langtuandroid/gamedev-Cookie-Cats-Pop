using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.TactilePrefs;

namespace TactileModules.TactileCloud.AssetBundles
{
	public static class AssetBundleSystemBuilder
	{
		public static AssetBundleSystem Build(CloudClientBase clientBase, AssetBundleManager.PauseLoadingCheck pauseLoadingCheck)
		{
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("AssetBundleManager", "PersistedState");
			LocalStorageJSONObject<PersistableState> localStorageObject = new LocalStorageJSONObject<PersistableState>(localStorageString);
			PersistableStateCache persistableStateHandler = new PersistableStateCache(localStorageObject);
			AnalyticsReporter analyticsReporter = new AnalyticsReporter(persistableStateHandler);
			AssetBundleDownloader assetBundleDownloader = new AssetBundleDownloader(analyticsReporter);
			AssetBundleManager assetBundleManager = new AssetBundleManager(clientBase, pauseLoadingCheck, assetBundleDownloader, persistableStateHandler);
			IAvailableAssetBundles availableAssetBundles = assetBundleManager;
			return new AssetBundleSystem(assetBundleManager, assetBundleDownloader, availableAssetBundles);
		}

		private const string PERSISTABLE_STATE_KEY = "PersistedState";

		private const string PERSISTABLE_STATE_DOMAIN = "AssetBundleManager";
	}
}
