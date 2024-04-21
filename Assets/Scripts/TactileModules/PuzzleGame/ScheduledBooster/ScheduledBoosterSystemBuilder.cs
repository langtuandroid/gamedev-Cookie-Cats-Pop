using System;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using TactileModules.PuzzleGame.ScheduledBooster.Views;

namespace TactileModules.PuzzleGame.ScheduledBooster
{
    public static class ScheduledBoosterSystemBuilder
    {
        public static IScheduledBoosterSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, ConfigurationManager configurationManager, IAnalytics tactileAnalytics, IScheduledBoosterProvider provider, IScheduledBoosterViewProvider viewProvider, IScheduledBoosterInventoryProvider inventoryProvider)
        {
            ScheduledBoosterDefinitions definitionsUtility = new ScheduledBoosterDefinitions(SingletonAsset<ScheduledBoosterSetup>.Instance.scheduledBoosterDefinitions);
            ScheduledBoosterFactory scheduledBoosterFactory = new ScheduledBoosterFactory(featureManager, definitionsUtility);
            ScheduledBoosters scheduledBoosters = new ScheduledBoosters(provider, viewProvider, inventoryProvider, tactileAnalytics);
            ScheduledBoosterHandler handler = new ScheduledBoosterHandler(featureManager, configurationManager, scheduledBoosters, scheduledBoosterFactory);
            return new ScheduledBoosterSystem(handler, scheduledBoosters, viewProvider);
        }
    }
}
