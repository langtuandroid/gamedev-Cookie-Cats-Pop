using System;
using Shared.FeatureManager.Module.Merging;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager.Analytics;
using TactileModules.FeatureManager.Cloud;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using TactileModules.TactileCloud.AssetBundles;
using TactileModules.TactileCloud.TargetingParameters;
using TactileModules.TactilePrefs;
using TactileModules.Timing.Interfaces;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.FeatureManager
{
	public static class FeatureManagerSystemBuilder
	{
		public static FeatureManagerSystem Build(CloudClientBase cloudClientBase, ITimingManager timeStampManager, IFeatureManagerProvider provider, ITargetingParameterFactory targetingParameterFactory, IAnalytics analytics, IAssetBundleSystem assetBundleSystem, IUserSettings userSettings, IFeatureSyncPoints syncPoints, IApplicationLifeCycleEvents applicationLifeCycleEvents)
		{
			FeatureAssetBundles featureAssetBundles = new FeatureAssetBundles(assetBundleSystem.AssetBundleDownloader, assetBundleSystem.AssetBundleManager);
			FeatureTypeUrlFileCachingFactory featureTypeUrlFileCachingFactory = new FeatureTypeUrlFileCachingFactory();
			UrlCacherFactory urlCacherFactory = new UrlCacherFactory(analytics);
			FeatureUrlFileCaching featureUrlFileCaching = new FeatureUrlFileCaching(featureTypeUrlFileCachingFactory, urlCacherFactory);
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("FeatureManager", "ReceivedFeatureEvents");
			LocalStorageJSONObject<FeatureReceivedEventLoggingState> localStorageObject = new LocalStorageJSONObject<FeatureReceivedEventLoggingState>(localStorageString);
			FeatureReceivedEventLoggingStateHandler featureReceivedEventLoggingStateHandler = new FeatureReceivedEventLoggingStateHandler(localStorageObject);
			FeatureManagerAnalytics featureManagerAnalytics = new FeatureManagerAnalytics(analytics, featureReceivedEventLoggingStateHandler);
			FeatureAvailabilityModel featureAvailability = new FeatureAvailabilityModel(featureManagerAnalytics);
			FeaturesCloud featuresCloud = new FeaturesCloud(cloudClientBase.cloudInterface, cloudClientBase, targetingParameterFactory);
			FeatureMergeUtil featureMergeUtil = new FeatureMergeUtil(featureManagerAnalytics);
			FeatureManager featureManager = new FeatureManager(timeStampManager, provider, userSettings, featureAssetBundles, featureUrlFileCaching, featureAvailability, featuresCloud, featureManagerAnalytics);
			FeatureManagerPersistableState.Initialize(featureMergeUtil, featureManagerAnalytics, featureManager);
			analytics.RegisterDecorator(new FeatureManagerBasicEventDecorator(featureManager));
			applicationLifeCycleEvents.ApplicationWillEnterForeground += featureManager.ApplicationWillEnterForeground;
			cloudClientBase.OnServerTimeUpdated += featureManager.ServerTimeUpdated;
			syncPoints.SafeForFeaturesToSync += featureManager.FadedToBlack;
			return new FeatureManagerSystem(featureManager);
		}

		private const string RECEIVED_FEATURE_EVENTS_KEY = "ReceivedFeatureEvents";

		private const string STORAGE_DOMAIN_NAMESPACE = "FeatureManager";
	}
}
