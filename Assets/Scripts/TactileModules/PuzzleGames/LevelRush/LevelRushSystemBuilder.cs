using System;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.LevelRush
{
    public static class LevelRushSystemBuilder
    {
        public static ILevelRushSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, ILevelRushNotificationProvider levelRushNotificationProvider, MapFacade mapFacade, MainProgressionManager mainProgressionManager, MapPopupManager mapPopupManager, ConfigurationManager configurationManager, InventoryManager inventoryManager)
        {
            ConfigGetter<LevelRushConfig> configGetter = new ConfigGetter<LevelRushConfig>(configurationManager);
            LevelRushFeatureHandler levelRushFeatureHandler = new LevelRushFeatureHandler(featureManager, configGetter, levelRushNotificationProvider);
            MainLevelsIndices mainLevelsIndices = new MainLevelsIndices(mainProgressionManager);
            LevelRushProgression progression = new LevelRushProgression(levelRushFeatureHandler, inventoryManager, mainLevelsIndices, configGetter, mainProgressionManager);
            LevelRushActivation levelRushActivation = new LevelRushActivation(levelRushFeatureHandler, featureManager, mainLevelsIndices, mainProgressionManager, configGetter);
            AssetModel assetModel = new AssetModel();
            MainMapPlugin mainMapPlugin = new MainMapPlugin(mainProgressionManager, levelRushActivation, progression, assetModel);
            mapFacade.MapPlugins.Add(mainMapPlugin);
            LevelRushControllerFactory controllerFactory = new LevelRushControllerFactory(levelRushActivation, mainMapPlugin, assetModel);
            mapPopupManager.RegisterPopupObject(new LevelRushStartupPopup(levelRushActivation, assetModel));
            mapPopupManager.RegisterPopupObject(new LevelRushStartedPopup(levelRushActivation, mainMapPlugin, assetModel));
            mapPopupManager.RegisterPopupObject(new LevelRushLocalStartedPopup(levelRushActivation, controllerFactory));
            mapPopupManager.RegisterPopupObject(new LevelRushRewardPopup(mainMapPlugin, levelRushActivation, progression, assetModel));
            mapPopupManager.RegisterPopupObject(new LevelRushEndedPopup(levelRushActivation));
            return new LevelRushSystem(levelRushFeatureHandler);
        }
    }
}
