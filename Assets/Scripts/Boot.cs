using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Code.Providers;
using CookieCatsPop.FrameworkImplementation.CrossPromotion;
using CookieCatsPop.FrameworkImplementation.NinjaUi;
using CookieCatsPop.UserSupport;
using NinjaUI;
using Shared.OneLifeChallenge;
using Shared.UserSettings.Module;
using Tactile;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Shop;
using Tactile.GardenGame.Story;
using Tactile.SagaCore;
using Tactile.XperiaGamesClub;
using TactileModules.Ads;
using TactileModules.Ads.Configuration;
using TactileModules.AgeInfo;
using TactileModules.CrossPromotion;
using TactileModules.FacebookExtras;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using TactileModules.GameCore.Audio;
using TactileModules.GameCore.Audio.Assets;
using TactileModules.GameCore.ButtonArea;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.MenuTutorial;
using TactileModules.GameCore.Rewards;
using TactileModules.GameCore.StreamingAssets;
using TactileModules.InstallTimeTracking;
using TactileModules.Inventory;
using TactileModules.Placements;
using TactileModules.Portraits;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.Ads;
using TactileModules.PuzzleGame.HotStreak;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank;
using TactileModules.PuzzleGame.PiggyBank.Models;
using TactileModules.PuzzleGame.PiggyBank.Templates;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.ReengagementRewards;
using TactileModules.PuzzleGame.ReengagementRewards.Popups;
using TactileModules.PuzzleGame.ScheduledBooster;
using TactileModules.PuzzleGame.SlidesAndLadders;
using TactileModules.PuzzleGame.ThemeHunt;
using TactileModules.PuzzleGames.Common.TargetingParameters;
using TactileModules.PuzzleGames.EndlessChallenge;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.GameCore.Analytics;
using TactileModules.PuzzleGames.LevelDash;
using TactileModules.PuzzleGames.LevelRush;
using TactileModules.PuzzleGames.StarTournament;
using TactileModules.PuzzleGames.StoryMapEvent;
using TactileModules.PuzzleGames.TreasureHunt;
using TactileModules.SagaCore;
using TactileModules.SagaGame.AdsForSagaGame;
using TactileModules.SideMapButtons;
using TactileModules.SpecialOffers;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffersForSideMapButtons;
using TactileModules.TactileCloud;
using TactileModules.TactileCloud.AssetBundles;
using TactileModules.TactileCloud.TargetingParameters;
using TactileModules.UserSupport;
using UnityEngine;

public class Boot : BootBase
{
    protected override IEnumerator RegisterManagers(ManagerRepository repository, IApplicationLifeCycleEvents applicationLifeCycleEvents)
    {
       
        Input.multiTouchEnabled = false;
        Boot.RegisterTouchConsole();
        yield return null;
        this.autoRotationEnabled = (Screen.orientation == ScreenOrientation.AutoRotation);
        Boot.IsRequestsBlocked += true;
        UIViewManager uiViewManager = repository.Register<UIViewManager>(UIViewManager.CreateInstance((string localizeKey) => L.Get(localizeKey), true, 10, 0.75f), null);
        GameEventManager gameEventManager = repository.Register<GameEventManager>(GameEventManager.CreateInstance(), null);
        PuzzleGame.Initialize<PuzzleGameImplementation>();

        TactileRequest.Configure(() => Boot.IsRequestsBlocked, 30f, 2);
        FacebookClient facebookClient = repository.Register<FacebookClient>(new FacebookClient(Constants.FACEBOOK_APP_ID, Constants.FACEBOOK_APP_NAMESPACE, Constants.FACEBOOK_URL_SUFFIX, Constants.FACEBOOK_OPENGRAPH_VERSION), null);
        RequestMetaDataProviderRegistry requestMetaDataProviderRegistry = new RequestMetaDataProviderRegistry();
        CloudClient cloudClient = repository.Register<CloudClient>(new CloudClient(facebookClient, requestMetaDataProviderRegistry), null);
        OneSignalManager oneSignalManager = repository.Register<OneSignalManager>(new OneSignalManager(Constants.ONESIGNAL_APP_ID, Constants.GCM_SENDER_ID, cloudClient), null);
        FacebookLoginManager facebookLoginManager = repository.Register<FacebookLoginManager>(new FacebookLoginManager(new FacebookLoginManagerProvider(), PuzzleGame.DialogViews), null);
        ConfigurationManager configurationManager = repository.Register<ConfigurationManager>(new ConfigurationManager(cloudClient), null);
        GameSessionManager gameSessionManager = repository.Register<GameSessionManager>(GameSessionManager.CreateInstance(new GameSessionManagerProvider()), null);
        MapPopupManager mapPopupManager = repository.Register<MapPopupManager>(MapPopupManager.CreateInstance(gameSessionManager, null), null);
        TimeStampManager timeStampManager = repository.Register<TimeStampManager>(new TimeStampManager(), null);
        InAppPurchaseManager iapManager = repository.Register<InAppPurchaseManager>(new InAppPurchaseManager(new DialogViewProvider(), facebookClient, cloudClient, configurationManager), null);

        Analytics analytics = repository.Register<Analytics>(new Analytics(cloudClient, configurationManager), null);


        TactileAnalytics tactileAnalytics = TactileAnalytics.Instance;
        ICloudUserSettingsProvider cloudClient2 = cloudClient;
        Action<UserSettingsManager> defaultSettingsHandler = new Action<UserSettingsManager>(this.DefaultSettingsHandler);

        UserSettingsManager userSettingsManager = repository.Register<UserSettingsManager>(UserSettingsManager.CreateInstance(cloudClient2, defaultSettingsHandler, new Action<Hashtable, Hashtable>(UserSettingsUpgrader.Upgrade)), null);
        repository.Register<AgeInfoManager>(new AgeInfoManager(uiViewManager, tactileAnalytics, new AgeInfoManagerProvider()), null);
        AssetBundleSystem assetBundleSystem = repository.Register<AssetBundleSystem>(AssetBundleSystemBuilder.Build(cloudClient, tactileAnalytics, new AssetBundleManager.PauseLoadingCheck(this.AreRequestsBlocked)), null);
        AssetBundleManager assetBundleManager = assetBundleSystem.AssetBundleManager;
        LevelDatabaseCollection levelDatabaseCollection = repository.Register<LevelDatabaseCollection>(new LevelDatabaseCollection(assetBundleManager, tactileAnalytics), null);
        MapStreamerCollection mapStreamerCollection = repository.Register<MapStreamerCollection>(new MapStreamerCollection(assetBundleManager, tactileAnalytics), null);
        repository.Register<EffectPool>(EffectPool.CreateInstance(), null);
        AudioManager audioManager = repository.Register<AudioManager>(AudioManager.CreateInstance(), null);
        GateManager gateManager = repository.Register<GateManager>(GateManager.CreateInstance(new GateManagerHelper()), null);
        MainProgressionManager mainProgressionManager = repository.Register<MainProgressionManager>(MainProgressionManager.CreateInstance(gateManager, new MainProgressionProvider(), levelDatabaseCollection, int.MaxValue), null);
        AdConfigurationProvider adConfigurationProvider = repository.Register<AdConfigurationProvider>(new AdConfigurationProvider(cloudClient, configurationManager, analytics), null);
        IInstallTime installTime = InstallTimeBuilder.Build();
        RewardedVideoPresenter rewardedVideoPresenter = repository.Register<RewardedVideoPresenter>(RewardedVideoSystemBuilder.Build(new MusicControlProvider(audioManager), adConfigurationProvider, tactileAnalytics), null);
        InterstitialPresenter interstitialPresenter = repository.Register<InterstitialPresenter>(InterstitialSystemBuilder.Build(adConfigurationProvider, new PayingUserProvider(new UserSettingsGetter<InAppPurchaseManager.PersistableState>(userSettingsManager)), installTime), null);
        AdManagerSystemBuilder.Build(adConfigurationProvider, mainProgressionManager, rewardedVideoPresenter, interstitialPresenter);

        SideMapButtonSystem sideButtonsSystem = SideMapButtonsSystemBuilder.Build();
        SpinnerViewController spinnerViewController = new SpinnerViewController(uiViewManager);
        BasicDialogViewController basicDialogViewController = new BasicDialogViewController(uiViewManager);
        UserProgressProvider userProgressProvider = new UserProgressProvider(mainProgressionManager);
        CrossPromotionSystemBuilder.Build(Constants.CROSS_PROMOTION_CAMPAIGN_CONTEXT, interstitialPresenter, rewardedVideoPresenter, gameSessionManager, uiViewManager, sideButtonsSystem, tactileAnalytics, spinnerViewController, basicDialogViewController, userProgressProvider);
        PuzzleTargetingParametersProvider puzzleTargetingParametersProvider = new PuzzleTargetingParametersProvider(mainProgressionManager, userSettingsManager, interstitialPresenter, analytics);
        EndlessChallengeTargetingParameters endlessChallengeTargetingParameters = new EndlessChallengeTargetingParameters(mainProgressionManager);
        TargetingParameterFactory targetingParameterFactory = new TargetingParameterFactory(cloudClient, installTime, new ITargetingParametersProvider[]
        {
            puzzleTargetingParametersProvider,
            endlessChallengeTargetingParameters
        });
        FeatureAssetBundles featureAssetBundles = new FeatureAssetBundles(assetBundleSystem.AssetBundleDownloader, assetBundleSystem.AssetBundleManager);
        PuzzleCoreCommon puzzleCoreCommon = PuzzleCoreCommonBuilder.BuildCommon(null);
        repository.Register<IFlowStack>(puzzleCoreCommon.FlowStack, null);
        repository.Register<IFullScreenManager>(puzzleCoreCommon.FullScreenManager, null);
        FeatureManagerSystem featureManagerSystem = FeatureManagerSystemBuilder.Build(cloudClient, timeStampManager, new FeatureManagerProvider(), targetingParameterFactory, tactileAnalytics, assetBundleSystem, userSettingsManager, new FeatureSyncPoints(puzzleCoreCommon.FullScreenManager), applicationLifeCycleEvents);
        FeatureManager featureManager = (FeatureManager)featureManagerSystem.FeatureManager;
        repository.Register<FeatureManager>(featureManager, null);
        LevelReleaseManager levelReleaseManager = new LevelReleaseManager(featureManager, mainProgressionManager, new LevelReleaseManagerProvider());
        HardLevelsManager hardLevelsManager = new HardLevelsManager(featureManager, new HardLevelsManagerProvider(), mainProgressionManager);
        OneLifeChallengeManager oneLifeChallengeManager = new OneLifeChallengeManager(featureManager, new OneLifeChallengeManagerProvider());
        RandomPortraitsAndNames randomPortraitsAndNames = new RandomPortraitsAndNames();
        SurveyManager surveyManager = repository.Register<SurveyManager>(new SurveyManager(featureManager, mapPopupManager, new SurveyManagerProvider()), null);
        IScheduledBoosterSystem scheduledBoosterSystem = repository.Register<IScheduledBoosterSystem>(ScheduledBoosterSystemBuilder.Build(featureManager, configurationManager, tactileAnalytics, new ScheduledBoosterProvider(), new ScheduledBoosterViewProvider(), new ScheduledBoosterInventoryProvider()), null);
        LeaderboardManager leaderboardManager = repository.Register<LeaderboardManager>(LeaderboardManager.CreateInstance(cloudClient), null);
        UserSettingsBackupManager userSettingsBackupManager = repository.Register<UserSettingsBackupManager>(UserSettingsBackupManager.CreateInstance(new UserSettingsBackupManagerProvider(), cloudClient.cloudInterface, timeStampManager), null);
        AdjustEventConstants adjustEventConstants = new AdjustEventConstants();
        AdjustTracking adjustTracking = new AdjustTracking();
        IInventorySystem inventorySystem = TactileModules.Inventory.InventorySystemBuilder.Build(tactileAnalytics, new AdjustInventoryTracking(adjustEventConstants), timeStampManager, new UnlimitedItemsProvider());
        repository.Register<IInventorySystem>(inventorySystem, null);
        InventoryManager inventoryManager = repository.Register<InventoryManager>(inventorySystem.InventoryManager, null);
        ClientMessageMetaDataProvider messageMetaDataProvider = new ClientMessageMetaDataProvider(userSettingsManager, mainProgressionManager);
        AttachmentsListener attachmentsListener = new AttachmentsListener(uiViewManager, inventoryManager, userSettingsManager);
        SupportedAttachments supportedAttachments = new SupportedAttachments();
        BackupRestorer backupRestorer = new BackupRestorer(userSettingsManager);
        UserSettingsBackupSummaryProvider userSettingsDetailsProvider = new UserSettingsBackupSummaryProvider(userSettingsManager, inventoryManager);
        UserSupportSystem userSupportSystem = UserSupportSystemBuilder.Build(attachmentsListener, facebookClient, oneSignalManager, messageMetaDataProvider, cloudClient, iapManager, mapPopupManager, uiViewManager, supportedAttachments, backupRestorer, userSettingsDetailsProvider, tactileAnalytics);
        repository.Register<UserSupportSystem>(userSupportSystem, null);
        CloudSynchronizer cloudSynchronizer = repository.Register<CloudSynchronizer>(new CloudSynchronizer(userSettingsManager, cloudClient, facebookClient, userSettingsBackupManager, configurationManager, assetBundleManager, leaderboardManager, featureManager, userSupportSystem.Synchronizer, () => Boot.IsRequestsBlocked), null);
        repository.Register<BonusDropManager>(Singleton<BonusDropManager>.CreateInstance(), null);
        repository.Register<LocalizationManager>(new LocalizationManager(assetBundleManager), null);
        repository.Register<TextureStreamerAssetBundleManager>(TextureStreamerAssetBundleManager.CreateInstance(new TextureStreamerAssetBundleManagerDataProvider(), assetBundleSystem.AssetBundleDownloader), null);
        ReengagementRewardProvider reengagementRewardProvider = new ReengagementRewardProvider(inventoryManager, gateManager, gameEventManager);
        ReengagementRewardManager reengagementRewardManager = repository.Register<ReengagementRewardManager>(new ReengagementRewardManager(mainProgressionManager, reengagementRewardProvider), null);
        mapPopupManager.RegisterPopupObject(new ReengagementRewardPopup(reengagementRewardManager, uiViewManager, reengagementRewardProvider));
        repository.Register<MapAssetBundleManager>(new MapAssetBundleManager(assetBundleManager, assetBundleSystem.AssetBundleDownloader), null);
        repository.Register<GameStatsManager>(new GameStatsManager(), null);
        repository.Register<DailyQuestManager>(DailyQuestManager.CreateInstance(new DailyQuestManagerProvider(), levelDatabaseCollection), null);
        SendLivesAtStartManager sendLivesAtStartManager = repository.Register<SendLivesAtStartManager>(new SendLivesAtStartManager(), null);
        repository.Register<FacebookRequestManager>(new FacebookRequestManager(cloudClient, facebookClient, () => Boot.IsRequestsBlocked, new Action<FacebookRequestData>(this.SuccessfulFacebookRequestSent), new FacebookRequestProvider(), new Func<FacebookRequestData, bool>(this.IsRequestAllowed)), null);
        repository.Register<VipManager>(VipManager.CreateInstance(), null);
        repository.Register<LikeUsManager>(LikeUsManager.CreateInstance(), null);
        repository.Register<ReviewManager>(new ReviewManager(configurationManager), null);
        repository.Register<LiveVersionUpdateManager>(new LiveVersionUpdateManager(), null);
        repository.Register<NotificationManager>(NotificationManager.CreateInstance(facebookClient), null);
        AchievementsManager achievementsManager = repository.Register<AchievementsManager>(new AchievementsManager(GameEventManager.Instance, new AchievementsHelper(), null), null);
        AchievementsManager achievementsManager2 = achievementsManager;

        achievementsManager2.OnAchievementCompleted += new Action<AchievementAsset>(AchievementsHelper.HandleMissionComplete);
        repository.Register<ThemeHuntManager>(ThemeHuntManagerBase.CreateInstance<ThemeHuntManager>(), null);
        XperiaGamesClubManager xperiaGamesClubManager = repository.Register<XperiaGamesClubManager>(new XperiaGamesClubManager(), null);
        repository.Register<XperiaGiftPopupManager>(new XperiaGiftPopupManager(xperiaGamesClubManager, new XperiaClubProvider()), null);
        repository.Register<BoosterManager>(BoosterManagerBase<BoosterManager>.CreateInstance(), null);
        repository.Register<UserCareManager>(UserCareManager.CreateInstance(), null);
        repository.Register<SeagullManager>(Singleton<SeagullManager>.CreateInstance(), null);
        GameImplementors.Initialize();
        PuzzleCoreCommonBuilder.BuildAnalytics(tactileAnalytics, configurationManager, inventoryManager, puzzleCoreCommon.FlowStack, FeatureManager.Instance, new AnalyticsContextProvider());
        UIController uiController = new UIController(uiViewManager);
        AssetsModel audioAssets = new AssetsModel();
        AudioDatabaseInjector audio = new AudioDatabaseInjector(audioAssets);
        RewardAreaModel rewardAreaModel = new RewardAreaModel();
        VisualInventory visualInventory = new VisualInventory(rewardAreaModel, audio, InventoryManager.Instance);
        IButtonAreaSystem buttonAreaSystem = ButtonAreaSystemBuilder.Build(uiController);
        ShopSystem shop = ShopSystemBuilder.Build((FlowStack)puzzleCoreCommon.FlowStack, uiController, buttonAreaSystem.Model, visualInventory, iapManager, configurationManager, userSettingsManager);
        ShopManager shopManager = shop.ShopManager;

        shopManager.ShopItemBought += new Action<ShopItem>(Boot.HandleShopItemBought);
        LivesManager livesManager = LivesManager.CreateInstance();
        Analytics.Instance.HookIntoGameCore(puzzleCoreCommon.FlowStack);
        LevelPlayingSystem levelPlayingSystem = LevelPlayingSystemBuilder.Build(livesManager, puzzleCoreCommon.FullScreenManager, new PlayLevelFactory(), uiViewManager);
        repository.Register<IPlayFlowFactory>(levelPlayingSystem.PlayFlowFactory, null);
        MainProgressionForAnalyticsProvider mainProgressionAnalyticsProvider = new MainProgressionForAnalyticsProvider(mainProgressionManager);
        LevelPlayingSystemBuilder.BuildAnalytics(tactileAnalytics, AdjustEventConstants.ADJUST_IO_MISSION_STARTED_EVENT_TOKEN, adjustTracking, mainProgressionAnalyticsProvider, levelPlayingSystem.PlayFlowEvents);
        PlacementSystem placementsSystem = PlacementSystemBuilder.Build(configurationManager, UIViewManager.Instance, tactileAnalytics);
        repository.Register<PlacementSystem>(placementsSystem, null);

        ISagaCoreSystem sagaCore = SagaCoreSystemBuilder.Build(puzzleCoreCommon.FlowStack, puzzleCoreCommon.FullScreenManager, levelPlayingSystem.PlayFlowFactory, mainProgressionManager, leaderboardManager, gateManager, cloudClient, VipManager.Instance, facebookClient, mapPopupManager, GameSessionManager.Instance, new StoryFlowProvider(), placementsSystem.PlacementRunner, tactileAnalytics, levelDatabaseCollection, mapStreamerCollection, hardLevelsManager);
        repository.Register<IMainMapFlowFactory>(sagaCore.MainMapFlowFactory, null);
        new LevelSessionIdPatchup(levelPlayingSystem.PlayFlowEvents);
        repository.Register<MainLeaderBoardScoresRecorder>(new MainLeaderBoardScoresRecorder(levelPlayingSystem.PlayFlowEvents, leaderboardManager, facebookLoginManager), null);
        new SynchronizerAtLevelEnd(levelPlayingSystem.PlayFlowEvents, cloudSynchronizer);
        new AdjustProgressionEventsLogger(levelPlayingSystem.PlayFlowEvents, adjustEventConstants, adjustTracking, gateManager);
        new BoosterSuggester(levelPlayingSystem.PlayFlowEvents);
        MusicSystemBuilder.Build(audioManager, puzzleCoreCommon.FlowStack);
        PiggyBankSystem piggyBankSystem = repository.Register<PiggyBankSystem>(PiggyBankSystemBuilder.Build(sagaCore.MapFacade, mainProgressionManager, new PiggyBankIAPProvider(shop.ShopManager, iapManager, new ConfigProvider<PiggyBankConfig>(configurationManager)), new PiggyBankMapProvider(), new ItemsProvider(), new PiggyBankProvider(userSettingsManager), userSettingsManager, configurationManager, iapManager, mapPopupManager), null);
        PlayablePostcardSystem playablePostcardSystem = repository.Register<PlayablePostcardSystem>(PlayablePostcardSystemBuilder.Build(featureManager, configurationManager, levelDatabaseCollection, mainProgressionManager, assetBundleManager, assetBundleSystem.AssetBundleDownloader, uiViewManager, tactileAnalytics, new PlayablePostcardProvider(), sagaCore.MapFacade, puzzleCoreCommon.FlowStack, puzzleCoreCommon.FullScreenManager, levelPlayingSystem.PlayFlowFactory), null);
        SideMapButtonSystem sideMapButtonSystem = SideMapButtonsSystemBuilder.Build();
        HotStreakManager hotStreakManager = new HotStreakManager(featureManager, new HotStreakProvider(), levelPlayingSystem.PlayFlowEvents);
        ITournamentSystem tournamentSystem = TournamentSystemBuilder.Build(sagaCore, levelPlayingSystem.PlayFlowFactory, puzzleCoreCommon.FullScreenManager, puzzleCoreCommon.FlowStack, cloudClient, levelDatabaseCollection, mapStreamerCollection, timeStampManager, new TournamentUI(), mapPopupManager, LivesManager.Instance);
        repository.Register<ITournamentSystem>(tournamentSystem, typeof(ITournamentSystem));
        repository.Register<TournamentCloudManager>(tournamentSystem.CloudManager, null);
        IDailyQuestSystem dailyQuestSystem = DailyQuestSystemBuilder.Build(DailyQuestManager.Instance, levelPlayingSystem.PlayFlowFactory, sagaCore.MapFacade);
        repository.Register<IDailyQuestSystem>(dailyQuestSystem, typeof(IDailyQuestSystem));
        FacebookPlacementsBuilder.Build(placementsSystem.PlacementRunnableRegistry, configurationManager, facebookLoginManager, mainProgressionManager, facebookClient, sendLivesAtStartManager);
        ILevelDashSystem levelDashSystem = LevelDashSystemBuilder.Build(featureManager, cloudClient, mapPopupManager, sagaCore, gameSessionManager, new LevelDashDataProvider(), new LevelDashMapAvatarModifierProvider(), new LevelDashViewProvider());
        repository.Register<ILevelDashSystem>(levelDashSystem, typeof(ILevelDashSystem));
        ILevelRushSystem levelRushSystem = LevelRushSystemBuilder.Build(featureManager, new LevelRushNotificationProvider(), sagaCore.MapFacade, mainProgressionManager, mapPopupManager, configurationManager, InventoryManager.Instance);
        IStarTournamentSystem starTournamentSystem = StarTournamentSystemBuilder.Build(featureManager, new StarTournamentManagerProvider(), cloudClient, sagaCore, levelPlayingSystem.PlayFlowEvents, mapPopupManager, mainProgressionManager, configurationManager, facebookClient, inventoryManager, randomPortraitsAndNames);
        TreasureHuntSystem treasureHuntSystem = TreasureHuntSystemBuilder.Build(featureManager, new TreasureHuntManagerProvider(), levelPlayingSystem.PlayFlowFactory, puzzleCoreCommon.FlowStack, puzzleCoreCommon.FullScreenManager, sagaCore.MapFacade);
        EndlessChallengeHandler endlessChallengeHandler = new EndlessChallengeHandler(featureManager, cloudClient, levelDatabaseCollection, configurationManager, uiViewManager, puzzleCoreCommon.FlowStack, levelPlayingSystem.PlayFlowFactory);
        SlidesAndLaddersSystem slidesAndLaddersSystem = repository.Register<SlidesAndLaddersSystem>(SlidesAndLaddersSystemBuilder.Build(featureManager, mapStreamerCollection, configurationManager, levelDatabaseCollection.GetLevelDatabase<SlidesAndLaddersLevelDatabase>("SlidesAndLadders"), new SlidesAndLaddersMapViewProvider(), new SlidesAndLaddersSave(), new SlidesAndLaddersInventory(), mainProgressionManager, livesManager, sagaCore.MapFacade, puzzleCoreCommon.FlowStack, puzzleCoreCommon.FullScreenManager, levelPlayingSystem.PlayFlowFactory), null);
        IOneLifeChallengeSystem onelifeChallengeSystem = OneLifeChallengeSystemBuilder.Build(oneLifeChallengeManager, sagaCore, levelPlayingSystem.PlayFlowFactory, puzzleCoreCommon.FullScreenManager, puzzleCoreCommon.FlowStack);
        repository.Register<IOneLifeChallengeSystem>(onelifeChallengeSystem, null);
        PropsManager propsManager = new PropsManager(userSettingsManager);
        StoryAudio storyAudio = new StoryAudio();
        StorySystem storySystem = StorySystemBuilder.Build((FlowStack)puzzleCoreCommon.FlowStack, visualInventory, uiController, buttonAreaSystem.Model, userSettingsManager, propsManager, configurationManager, shop.ShopViewFlowFactory, storyAudio);
        repository.Register<StoryManager>(storySystem.StoryManager, null);
        MainMapStateFactory mainMapStateFactory = new MainMapStateFactory(uiController, puzzleCoreCommon.FullScreenManager, storySystem.StoryControllerFactory, propsManager, storySystem.StoryManager, placementsSystem.PlacementRunner, userSettingsManager, (FlowStack)puzzleCoreCommon.FlowStack);
        repository.Register<MainMapStateFactory>(mainMapStateFactory, null);
        StreamingAssetsSystemBuilder.Build(assetBundleSystem.AssetBundleManager, assetBundleSystem.AssetBundleDownloader);
        IStoryMapEventSystem storyMapEventSystem = StoryMapEventSystemBuilder.Build(levelPlayingSystem, storySystem.StoryManager, storySystem.BrowseTasksFactory, inventoryManager, mainMapStateFactory, sideMapButtonSystem, configurationManager, featureManager, placementsSystem.PlacementRunnableRegistry, puzzleCoreCommon.FlowStack, mainProgressionManager, uiViewManager, tactileAnalytics, new StoryMapEventNotificationProvider());
        NinjaUISystem ninjaUiSystem = NinjaUISystemBuilder.Build(tactileAnalytics);
        SpecialOffersSystem specialOffersSystem = SpecialOffersSystemBuilder.Build(iapManager, featureManager, tactileAnalytics, new SpecialOffersMainProgressionProvider(mainProgressionManager), configurationManager, placementsSystem.PlacementRunnableRegistry, inventoryManager, shop.ShopManager, PlacementIdentifier.PostAnimateAvatar);
        SpecialOffersForSideMapButtonsSystemBuilder.Build(specialOffersSystem, sideMapButtonSystem, uiViewManager);
        List<IFeatureTypeHandler> featureHandlers = new List<IFeatureTypeHandler>();
        featureHandlers.Add(levelRushSystem.LevelRushFeatureHandler);
        featureHandlers.Add(hardLevelsManager);
        featureHandlers.Add(treasureHuntSystem.Manager);
        featureHandlers.Add(oneLifeChallengeManager);
        featureHandlers.Add(levelReleaseManager);
        featureHandlers.Add(starTournamentSystem.Manager);
        featureHandlers.Add(hotStreakManager);
        featureHandlers.Add(levelDashSystem.LevelDashManager);
        featureHandlers.Add(endlessChallengeHandler);
        featureHandlers.Add(scheduledBoosterSystem.Handler);
        featureHandlers.Add(slidesAndLaddersSystem.Handler);
        featureHandlers.Add(playablePostcardSystem.Handler);
        featureHandlers.Add(storyMapEventSystem.StoryMapEventFeatureHandler);
        featureHandlers.Add(specialOffersSystem.FeatureHandler);
        foreach (IFeatureTypeHandler item in surveyManager.GetSurveyHandlers())
        {
            featureHandlers.Add(item);
        }
        this.RegisterFeatureHandlers(featureHandlers);
        MenuTutorialsSystemBuilder.Build(uiController, new TutorialProgression(storySystem.StoryManager), () => storySystem.StoryManager.FirstIntroCompleted && storyMapEventSystem.StoryMapEventFeatureHandler.HasActiveFeature());
        Boot.IsRequestsBlocked += false;
        yield break;
    }

    private void RegisterFeatureHandlers(List<IFeatureTypeHandler> handlers)
    {
        FeatureManager.Instance.RegisterFeatureHandlers(handlers.ToArray());
    }

    protected override void RegisterCloudSynchronizables()
    {
    }

    protected override void BootCompleted()
    {
        Flow.Start(new BootFlow());
    }

    private void DefaultSettingsHandler(UserSettingsManager mgr)
    {
        InventoryManager.PersistableState settings = mgr.GetSettings<InventoryManager.PersistableState>();
        settings.items["Life"] = ConfigurationManager.Get<LivesConfig>().NotLoggedInMaxlives;
        settings.items["TournamentLife"] = ConfigurationManager.Get<TournamentConfig>().LifeRegenerationMaxCount;
    }

    public static string ConstructCrashData()
    {
        Hashtable hashtable = new Hashtable();
        if (NinjaUIDebugData.DebugData != null)
        {
            hashtable["NinjaUIDebugData"] = NinjaUIDebugData.DebugData;
        }
        return hashtable.toJson();
    }

    private static void HandleShopItemBought(ShopItem shopItem)
    {
        string type = shopItem.Type;
        if (type != null)
        {
            if (type == "ShopItemVip")
            {
                VipManager.Instance.UserBoughtVipSubscription();
            }
        }
    }

    private bool IsRequestAllowed(FacebookRequestData request)
    {
        return GateManager.Instance != null && request != null && (!(request.RequestType == "key") || (GateManager.Instance.PlayerOnGate && !GateManager.Instance.IsUserAlreadyAGateKeyGiver(request.RequestSenderFacebookId)));
    }

    private void SuccessfulFacebookRequestSent(FacebookRequestData data)
    {
        if (data is FacebookKeyRequestData)
        {
            GameEventManager.Instance.Emit(10, data, 1);
        }
        else if (data is FacebookLifeRequestData)
        {
            GameEventManager.Instance.Emit(11, data, 1);
        }
        else if (data is FacebookTournamentLifeRequestData)
        {
            GameEventManager.Instance.Emit(12, data, 1);
        }
    }

    private bool AreRequestsBlocked()
    {
        return Boot.IsRequestsBlocked;
    }

    private static void RegisterTouchConsole()
    {
    }

    public static NestedEnabler IsRequestsBlocked = new NestedEnabler(false);

    private bool autoRotationEnabled;


}
