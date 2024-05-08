using System;
using Tactile;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager;
using TactileModules.Placements;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SideMapButtons;
using TactileModules.TactilePrefs;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
    public static class StoryMapEventSystemBuilder
    {
        public static IStoryMapEventSystem Build(LevelPlayingSystem levelPlayingSystem, IStoryManager storyManager, BrowseTasksFactory browseTasksFactory, InventoryManager inventoryManager, MainMapStateFactory mainMapStateFactory, SideMapButtonSystem sideMapButtonSystem, ConfigurationManager configurationManager, TactileModules.FeatureManager.FeatureManager featureManager, IPlacementRunnableRegistry placementRunnableRegistry, IFlowStack flowStack, IMainProgression mainProgression, UIViewManager uiViewManager, IStoryMapEventNotificationProvider storyMapEventNotificationProvider)
        {
            ConfigGetter<StoryMapEventConfig> configGetter = new ConfigGetter<StoryMapEventConfig>(configurationManager);
            StoryMapEventFeatureHandler storyMapEventFeatureHandler = new StoryMapEventFeatureHandler(configGetter, storyMapEventNotificationProvider);
            StoryMapEventActivation featureActivation = new StoryMapEventActivation(storyMapEventFeatureHandler, featureManager, configGetter, mainProgression);
            ViewFactory viewFactory = new ViewFactory();
            CurrencyAvailability currencyAvailability = new CurrencyAvailability(mainProgression, featureActivation, storyManager, inventoryManager);
            MainMapStateProvider mainMapStateProvider = new MainMapStateProvider(currencyAvailability);
            FlowFactory flowFactory = new FlowFactory(flowStack, mainMapStateFactory, mainMapStateProvider);
            ChapterCompleteFlow chapterCompleteFlow = new ChapterCompleteFlow(storyManager, configGetter, viewFactory, uiViewManager, inventoryManager);
            string domainNamespace = "TactileModules.StoryMapEvent.Model";
            string key = "ReminderCooldown";
            LocalStorageJSONObject<ReminderTimeStamp> reminderTimeStampStorageObject = new LocalStorageJSONObject<ReminderTimeStamp>(new PlayerPrefsSignedString(domainNamespace, key));
            ReminderCooldown reminderCooldown = new ReminderCooldown(reminderTimeStampStorageObject, featureManager, configGetter);
            placementRunnableRegistry.RegisterRunnable(new PlacementRunnable(flowFactory, featureActivation, viewFactory, reminderCooldown, storyManager), PlacementIdentifier.PostAnimateAvatar, PlacementBehavior.Skippable);
            new HudController(flowStack, browseTasksFactory, storyManager, inventoryManager, viewFactory, uiViewManager);
            CurrencyCollector currencyCollector = new CurrencyCollector(levelPlayingSystem.PlayFlowEvents, inventoryManager, currencyAvailability, storyManager);
            new CurrencyOverlayLifecycleHandler(currencyAvailability, inventoryManager);
            new LevelStartAddonLifecycleHandler(currencyCollector, currencyAvailability, featureActivation, flowFactory, flowStack);
            new CurrencyRewardLifecycleHandler(currencyCollector);
            new CountdownLifecycleHandler(featureActivation);
            new AnalyticsEventLogger(storyManager, inventoryManager);
            SideButtonControllerFactory controllerProvider = new SideButtonControllerFactory(viewFactory, flowFactory, storyManager, inventoryManager, featureActivation, reminderCooldown);
            sideMapButtonSystem.Registry.Register(controllerProvider);
            return new StoryMapEventSystem(storyMapEventFeatureHandler);
        }
    }
}
