using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
    public class StoryMapEventActivation : IStoryMapEventActivation
    {
        public StoryMapEventActivation(IStoryMapEventFeatureHandler eventFeatureHandler, TactileModules.FeatureManager.FeatureManager featureManager, IConfigGetter<StoryMapEventConfig> configGetter, IMainProgression mainProgression)
        {
            this.eventFeatureHandler = eventFeatureHandler;
            this.featureManager = featureManager;
            this.configGetter = configGetter;
            this.mainProgression = mainProgression;
        }

        public void ActivateStoryMap()
        {
            FeatureData feature = this.featureManager.GetFeature(this.eventFeatureHandler);
            this.featureManager.ActivateFeature(this.eventFeatureHandler, feature);
        }

        public void DeactivateStoryMap()
        {
            this.featureManager.DeactivateFeature(this.eventFeatureHandler);
        }

        public bool ShouldActivateStoryMap()
        {
            StoryMapEventConfig storyMapEventConfig = this.configGetter.Get();
            return this.mainProgression.GetFarthestCompletedLevelHumanNumber() >= storyMapEventConfig.LevelRequired && this.featureManager.CanActivateFeature(this.eventFeatureHandler);
        }

        public bool ShouldDeactivateStoryMap()
        {
            return this.featureManager.ShouldDeactivateFeature(this.eventFeatureHandler);
        }

        public bool HasActiveFeature()
        {
            return this.eventFeatureHandler.HasActiveFeature();
        }

        public bool IsFeatureEnabledInConfiguration()
        {
            StoryMapEventConfig storyMapEventConfig = this.configGetter.Get();
            return storyMapEventConfig.FeatureEnabled;
        }

        public int GetSecondsLeft()
        {
            return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this.eventFeatureHandler);
        }

        private readonly IStoryMapEventFeatureHandler eventFeatureHandler;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly IConfigGetter<StoryMapEventConfig> configGetter;

        private readonly IMainProgression mainProgression;
    }
}
