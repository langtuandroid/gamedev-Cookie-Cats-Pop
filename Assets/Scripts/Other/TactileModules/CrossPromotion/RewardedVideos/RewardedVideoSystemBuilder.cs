using System;
using Fibers;
using TactileModules.Ads;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.Cloud.RequestState;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.General.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.RewardedVideos.Cloud;
using TactileModules.CrossPromotion.RewardedVideos.ViewControllers;
using TactileModules.CrossPromotion.RewardedVideos.ViewFactories;
using TactileModules.CrossPromotion.TemplateAssets.GeneratedScript;
using TactileModules.NinjaUi.SharedViewControllers;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.CrossPromotion.RewardedVideos
{
	public static class RewardedVideoSystemBuilder
	{
		public static void InitializeAndRegisterRewardedVideoProvider(IRewardedVideoPresenter rewardedVideoPresenter, IAdServerCloudInterface cloudInterface, ICloudResponseParser cloudResponseParser, ICrossPromotionAdFactory crossPromotionAdFactory, IGeneralDataRetriever generalDataRetriever, IGameSessionManager gameSessionManager, ITactileDateTime dateTimeGetter, IViewFactory viewFactory, IViewPresenter uiViewManager, CrossPromotionAnalyticsDataFactory analyticsDataFactory, IAnalytics analytics, ISpinnerViewController spinnerViewController, IBasicDialogViewController basicDialogViewController, IUserProgressProvider userProgressProvider)
		{
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("CrossPromotion", "RewardedVideoRequestStateDataStorage");
			LocalStorageJSONObject<RequestState> localStorageObject = new LocalStorageJSONObject<RequestState>(localStorageString);
			RequestStateHandler requestStateHandler = new RequestStateHandler(localStorageObject);
			RewardedVideoCloud cloud = new RewardedVideoCloud(cloudInterface, cloudResponseParser, requestStateHandler, dateTimeGetter);
			PlayerPrefsSignedString localStorageString2 = new PlayerPrefsSignedString("CrossPromotion", "RewardedVideoMetaDataStorage");
			LocalStorageJSONObject<CrossPromotionAdRetrieverData> storage = new LocalStorageJSONObject<CrossPromotionAdRetrieverData>(localStorageString2);
			AdSession session = new AdSession(AdType.Rewarded, generalDataRetriever, gameSessionManager);
			CrossPromotionAnalyticsEventFactory analyticsEventFactory = new CrossPromotionAnalyticsEventFactory(AdType.Rewarded, analyticsDataFactory);
			CrossPromotionAdRetriever crossPromotionAdRetriever = new CrossPromotionAdRetriever(crossPromotionAdFactory, session, cloud, storage, generalDataRetriever, analyticsEventFactory, dateTimeGetter, userProgressProvider);
			CrossPromotionAdUpdater videoUpdater = new CrossPromotionAdUpdater(crossPromotionAdRetriever, generalDataRetriever, dateTimeGetter, requestStateHandler);
			RewardedVideoViewFactory viewFactory2 = new RewardedVideoViewFactory(viewFactory, spinnerViewController, basicDialogViewController);
			RewardedVideoControllerFactory controllerFactory = new RewardedVideoControllerFactory(crossPromotionAdRetriever, videoUpdater, viewFactory2, uiViewManager, analyticsEventFactory, analytics, new Fiber(), new TactileDateTime());
			RewardedVideoProvider provider = new RewardedVideoProvider(crossPromotionAdRetriever, videoUpdater, controllerFactory);
			rewardedVideoPresenter.RegisterRewardedVideoProvider(provider);
		}

		private const AdType TYPE = AdType.Rewarded;

		private const string CROSS_PROMOTION_DOMAIN = "CrossPromotion";

		private const string REWARDED_VIDEO_METADATA_STORAGE_KEY = "RewardedVideoMetaDataStorage";

		private const string REWARDED_VIDEO_REQUEST_STATE_STORAGE_KEY = "RewardedVideoRequestStateDataStorage";
	}
}
