using System;
using TactileModules.Ads;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;
using TactileModules.CrossPromotion.General.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.General.LimitedUrlCaching;
using TactileModules.CrossPromotion.General.LimitedUrlCaching.Data;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.CrossPromotion.Interstitials;
using TactileModules.CrossPromotion.Interstitials.ViewControllers;
using TactileModules.CrossPromotion.RewardedVideos;
using TactileModules.CrossPromotion.TactileHub;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.CrossPromotion.TemplateAssets.GeneratedScript;
using TactileModules.NinjaUi.SharedViewControllers;
using TactileModules.RuntimeTools;
using TactileModules.RuntimeTools.Orientation;
using TactileModules.SideMapButtons;
using TactileModules.TactilePrefs;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.CrossPromotion
{
	public static class CrossPromotionSystemBuilder
	{
		public static CrossPromotionSystem Build(string currentGameId, IInterstitialPresenter interstitialPresenter, IRewardedVideoPresenter rewardedVideoPresenter, IGameSessionManager gameSessionManager, IUIViewManager uiViewManager, SideMapButtonSystem sideButtonsSystem, IAnalytics analytics, ISpinnerViewController spinnerViewController, IBasicDialogViewController basicDialogViewController, IUserProgressProvider userProgressProvider)
		{
			PromotedGameUtilityFactory promotedGameUtilityFactory = new PromotedGameUtilityFactory();
			IPromotedGameUtility promotedGameUtility = promotedGameUtilityFactory.Create(currentGameId);
			PromotedGameLauncher promotedGameLauncher = new PromotedGameLauncher(promotedGameUtility);
			UrlCacherFactory urlCacherFactory = new UrlCacherFactory(analytics);
			TactileDateTime dateTimeGetter = new TactileDateTime();
			IUrlCacher urlCacher = urlCacherFactory.Create("CrossPromotion/Texture");
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("CrossPromotion", "TextureCacherStorage");
			LocalStorageJSONObject<LimitedUrlCacherData> storage = new LocalStorageJSONObject<LimitedUrlCacherData>(localStorageString);
			LimitedUrlCacher textureCacher = new LimitedUrlCacher(urlCacher, 5, storage, dateTimeGetter);
			IUrlCacher urlCacher2 = urlCacherFactory.Create("CrossPromotion/Video");
			PlayerPrefsSignedString localStorageString2 = new PlayerPrefsSignedString("CrossPromotion", "VideoCacherStorage");
			LocalStorageJSONObject<LimitedUrlCacherData> storage2 = new LocalStorageJSONObject<LimitedUrlCacherData>(localStorageString2);
			LimitedUrlCacher videoCacher = new LimitedUrlCacher(urlCacher2, 2, storage2, dateTimeGetter);
			LimitedUrlCacherRetriever adCacherRetriever = new LimitedUrlCacherRetriever(textureCacher, videoCacher);
			AdServerCloudInterface cloudInterface = new AdServerCloudInterface();
			TextureLoader textureLoader = new TextureLoader();
			CloudResponseParser cloudResponseParser = new CloudResponseParser(analytics);
			GeneralDataCloud crossPromotionGeneralDataCloud = new GeneralDataCloud(cloudInterface, cloudResponseParser);
			HubGameFactory hubGameFactory = new HubGameFactory(promotedGameLauncher, textureLoader, spinnerViewController);
			PlayerPrefsSignedString localStorageString3 = new PlayerPrefsSignedString("CrossPromotion", "GeneralDataStorage");
			LocalStorageJSONObject<CrossPromotionGeneralData> localStorage = new LocalStorageJSONObject<CrossPromotionGeneralData>(localStorageString3);
			GeneralDataRetriever generalDataRetriever = new GeneralDataRetriever(crossPromotionGeneralDataCloud, localStorage, urlCacher, hubGameFactory, promotedGameLauncher);
			TactileHubSystemBuilder.Build(generalDataRetriever, uiViewManager);
			ScreenOrientationGetter screenOrientationGetter = new ScreenOrientationGetter();
			AdCreativeSelector adCreativeSelector = new AdCreativeSelector(screenOrientationGetter);
			CrossPromotionAnalyticsDataFactory analyticsDataFactory = new CrossPromotionAnalyticsDataFactory(screenOrientationGetter, adCreativeSelector);
			CrossPromotionAdFactory crossPromotionAdFactory = new CrossPromotionAdFactory(adCacherRetriever, adCreativeSelector, promotedGameLauncher, generalDataRetriever, dateTimeGetter, textureLoader, analytics);
			ViewFactory viewFactory = new ViewFactory();
			IInterstitialControllerFactory interstitialControllerFactory = TactileModules.CrossPromotion.Interstitials.InterstitialSystemBuilder.InitializeAndRegisterRewardedInterstitialProvider(interstitialPresenter, cloudInterface, cloudResponseParser, crossPromotionAdFactory, generalDataRetriever, gameSessionManager, dateTimeGetter, viewFactory, uiViewManager, sideButtonsSystem, analyticsDataFactory, spinnerViewController, userProgressProvider);
			TactileModules.CrossPromotion.RewardedVideos.RewardedVideoSystemBuilder.InitializeAndRegisterRewardedVideoProvider(rewardedVideoPresenter, cloudInterface, cloudResponseParser, crossPromotionAdFactory, generalDataRetriever, gameSessionManager, dateTimeGetter, viewFactory, uiViewManager, analyticsDataFactory, analytics, spinnerViewController, basicDialogViewController, userProgressProvider);
			return new CrossPromotionSystem(interstitialControllerFactory);
		}

		private const string CROSS_PROMOTION_DOMAIN = "CrossPromotion";

		private const string TEXTURE_STORAGE_KEY = "TextureCacherStorage";

		private const string VIDEO_STORAGE_KEY = "VideoCacherStorage";

		private const string GENERAL_DATA_STORAGE_KEY = "GeneralDataStorage";

		private const string CROSS_PROMOTION_TEXTURE_PATH = "CrossPromotion/Texture";

		private const string CROSS_PROMOTION_VIDEO_PATH = "CrossPromotion/Video";
	}
}
