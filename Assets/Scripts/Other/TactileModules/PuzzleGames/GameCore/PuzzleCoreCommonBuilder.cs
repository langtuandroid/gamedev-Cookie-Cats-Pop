using System;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.GameCore.Analytics;

namespace TactileModules.PuzzleGames.GameCore
{
    public static class PuzzleCoreCommonBuilder
    {
        public static PuzzleCoreCommon BuildCommon([CanBeNull] IFullScreenTransition fullScreenTransition)
        {
            FlowStack flowStack = new FlowStack();
            if (fullScreenTransition == null)
            {
                fullScreenTransition = new DipToBlackFullScreenTransition();
            }
            FullScreenManager fullScreenManager = new FullScreenManager(fullScreenTransition);
            return new PuzzleCoreCommon
            {
                FlowStack = flowStack,
                FullScreenManager = fullScreenManager
            };
        }

        public static void BuildAnalytics(IConfigurationManager configurationManager, InventoryManager inventoryManager, IFlowStack flowStack, TactileModules.FeatureManager.FeatureManager featureManager)
        {
            
        }
    }
}
