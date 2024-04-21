using System;
using TactileModules.FeatureManager;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public static class TreasureHuntSystemBuilder
    {
        public static TreasureHuntSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, ITreasureHuntProvider provider, IPlayFlowFactory playLevelFacade, IFlowStack flowStack, IFullScreenManager fullScreenManager, MapFacade mapFacade)
        {
            TreasureHuntManager treasureHuntManager = new TreasureHuntManager(featureManager, provider, playLevelFacade, flowStack, fullScreenManager, mapFacade);
            new TreasureHuntEndPopup(treasureHuntManager);
            new TreasureHuntSessionStartPopup(treasureHuntManager);
            new TreasureHuntStartPopup(treasureHuntManager, featureManager);
            return new TreasureHuntSystem(treasureHuntManager);
        }
    }
}
