using System;
using TactileModules.Ads;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud.CloudInterface;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.Cloud.RequestState;
using TactileModules.CrossPromotion.Cloud.ResponseParsing;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.General.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.Interstitials.Cloud;
using TactileModules.CrossPromotion.Interstitials.ViewControllers;
using TactileModules.CrossPromotion.Interstitials.ViewFactories;
using TactileModules.CrossPromotion.TemplateAssets.GeneratedScript;
using TactileModules.NinjaUi.SharedViewControllers;
using TactileModules.RuntimeTools;
using TactileModules.SideMapButtons;
using TactileModules.TactilePrefs;

namespace TactileModules.CrossPromotion.Interstitials
{
	public static class InterstitialSystemBuilder
	{
		public static IInterstitialControllerFactory InitializeAndRegisterRewardedInterstitialProvider(IInterstitialPresenter interstitialProviderManager, IAdServerCloudInterface cloudInterface, ICloudResponseParser cloudResponseParser, ICrossPromotionAdFactory crossPromotionAdFactory, GeneralDataRetriever generalDataRetriever, IGameSessionManager gameSessionManager, ITactileDateTime dateTimeGetter, IViewFactory viewFactory, IUIViewManager uiViewManager, SideMapButtonSystem sideButtonsSystem, ICrossPromotionAnalyticsDataFactory analyticsDataFactory, ISpinnerViewController spinnerViewController, IUserProgressProvider userProgressProvider)
		{
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("CrossPromotion", "InterstitialRequestStateDataStorage");
			LocalStorageJSONObject<RequestState> localStorageObject = new LocalStorageJSONObject<RequestState>(localStorageString);
			RequestStateHandler requestStateHandler = new RequestStateHandler(localStorageObject);
			InterstitialCloud cloud = new InterstitialCloud(cloudInterface, cloudResponseParser, requestStateHandler, dateTimeGetter);
			PlayerPrefsSignedString localStorageString2 = new PlayerPrefsSignedString("CrossPromotion", "InterstitialMetaDataStorage");
			LocalStorageJSONObject<CrossPromotionAdRetrieverData> storage = new LocalStorageJSONObject<CrossPromotionAdRetrieverData>(localStorageString2);
			AdSession session = new AdSession(AdType.Interstitial, generalDataRetriever, gameSessionManager);
			CrossPromotionAnalyticsEventFactory analyticsEventFactory = new CrossPromotionAnalyticsEventFactory(AdType.Interstitial, analyticsDataFactory);
			CrossPromotionAdRetriever crossPromotionAdRetriever = new CrossPromotionAdRetriever(crossPromotionAdFactory, session, cloud, storage, generalDataRetriever, analyticsEventFactory, dateTimeGetter, userProgressProvider);
			CrossPromotionInterstitialViewFactory viewFactory2 = new CrossPromotionInterstitialViewFactory(viewFactory, spinnerViewController);
			CrossPromotionAdUpdater crossPromotionAdUpdater = new CrossPromotionAdUpdater(crossPromotionAdRetriever, generalDataRetriever, dateTimeGetter, requestStateHandler);
			InterstitialControllerFactory interstitialControllerFactory = new InterstitialControllerFactory(uiViewManager, viewFactory2, crossPromotionAdRetriever, crossPromotionAdUpdater, interstitialProviderManager);
			sideButtonsSystem.Registry.Register(interstitialControllerFactory);
			InterstitialProvider provider = new InterstitialProvider(crossPromotionAdRetriever, crossPromotionAdUpdater, interstitialControllerFactory);
			interstitialProviderManager.Register(provider);
			return interstitialControllerFactory;
		}

		private const AdType TYPE = AdType.Interstitial;

		private const string CROSS_PROMOTION_DOMAIN = "CrossPromotion";

		private const string INTERSTITIAL_METADATA_STORAGE_KEY = "InterstitialMetaDataStorage";

		private const string INTERSTITIAL_REQUEST_STATE_STORAGE_KEY = "InterstitialRequestStateDataStorage";
	}
}
