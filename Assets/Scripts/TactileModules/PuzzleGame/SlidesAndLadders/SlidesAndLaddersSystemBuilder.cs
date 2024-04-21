using System;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.SlidesAndLadders.Analytics;
using TactileModules.PuzzleGame.SlidesAndLadders.Controllers;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.Popups;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.Lives;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.SlidesAndLadders
{
    public static class SlidesAndLaddersSystemBuilder
    {
        public static SlidesAndLaddersSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, MapStreamerCollection mapStreamerCollection, ConfigurationManager configurationManager, ISlidesAndLaddersLevelDatabase levelDatabaseCollection, ISlidesAndLaddersMapViewProvider slidesAndLaddersMapViewProvider, ISlidesAndLaddersSave save, ISlidesAndLaddersInventory inventory, IMainProgression mainProgressionManager, ILivesManager lives, MapFacade mapFacade, IFlowStack flowStack, IFullScreenManager fullScreenManager, IPlayFlowFactory playLevelFacade)
        {
            ConfigProvider<SlidesAndLaddersConfig> config = new ConfigProvider<SlidesAndLaddersConfig>(configurationManager);
            SlidesAndLaddersHandler slidesAndLaddersHandler = new SlidesAndLaddersHandler(featureManager, mapStreamerCollection, slidesAndLaddersMapViewProvider, levelDatabaseCollection, save, config);
            new SlidesAndLaddersStartPopup(featureManager, slidesAndLaddersHandler, config, mainProgressionManager);
            new SlidesAndLaddersSessionStartPopup(featureManager, slidesAndLaddersHandler);
            new SlidesAndLaddersEndPopup(featureManager, slidesAndLaddersHandler);
            SlidesAndLaddersControllerFactory slidesAndLaddersControllerFactory = new SlidesAndLaddersControllerFactory(slidesAndLaddersHandler, levelDatabaseCollection, inventory, save, lives, configurationManager, playLevelFacade, mapFacade, fullScreenManager, flowStack);
            new SlidesAndLaddersWheelAnalytics(flowStack, levelDatabaseCollection, slidesAndLaddersControllerFactory.GetFeatureProgression(), slidesAndLaddersControllerFactory.GetRewards());
            new SlidesAndLaddersGameSessionAnalytics(flowStack, levelDatabaseCollection);
            return new SlidesAndLaddersSystem(slidesAndLaddersHandler, slidesAndLaddersControllerFactory);
        }
    }
}
