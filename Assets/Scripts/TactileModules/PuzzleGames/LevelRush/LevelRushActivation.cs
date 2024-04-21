using System;
using System.Diagnostics;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.LevelRush
{
    public class LevelRushActivation : ILevelRushActivation
    {
        public LevelRushActivation(ILevelRushFeatureHandler featureHandler, TactileModules.FeatureManager.FeatureManager featureManager, IMainLevelsIndices mainLevelsIndices, IMainProgression mainProgression, IConfigGetter<LevelRushConfig> configGetter)
        {
            this.featureManager = featureManager;
            this.mainLevelsIndices = mainLevelsIndices;
            this.mainProgression = mainProgression;
            this.configGetter = configGetter;
            this.featureHandler = featureHandler;
            featureManager.OnFeatureDeactivated += this.HandleFeatureDeactivated;
        }

        ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<ActivatedFeatureInstanceData> FeatureDeactivated;



        private void HandleFeatureDeactivated(ActivatedFeatureInstanceData data)
        {
            if (data.FeatureData.Type == "level-rush")
            {
                this.FeatureDeactivated(data);
            }
        }

        public void ActivateLevelRush()
        {
            FeatureData feature = this.featureManager.GetFeature(this.featureHandler);
            this.ActivateLevelRush(feature);
        }

        public void ActivateLocalLevelRush()
        {
            ActivatedFeatureInstanceData initialLevelRushValues = this.featureManager.ActivateLocalFeature(this.featureHandler, this.Config.MaxPlayTimeDurationLocal);
            this.SetInitialLevelRushValues(initialLevelRushValues);
        }

        public void DeactivateLevelRush()
        {
            this.featureManager.DeactivateFeature(this.featureHandler);
        }

        public bool ShouldDeactivateLevelRush()
        {
            return this.featureManager.ShouldDeactivateFeature(this.featureHandler);
        }

        public bool HasPlayerAnyProgress()
        {
            return this.InstanceData.Progress > 0;
        }

        public bool HasActiveFeature()
        {
            return this.featureHandler.HasActiveFeature();
        }

        public bool FeatureEnabled()
        {
            return this.Config.FeatureEnabled;
        }

        public bool ShouldActivateLevelRush()
        {
            if (!this.Config.FeatureEnabled)
            {
                return false;
            }
            int humanNumberFromLevelIndex = this.mainLevelsIndices.GetHumanNumberFromLevelIndex(this.mainProgression.GetFarthestUnlockedLevelIndex());
            return humanNumberFromLevelIndex >= this.Config.LevelRequired && this.mainProgression.GetFarthestUnlockedLevelIndex() + this.Config.LevelRushRewards[0].LevelIndex < this.mainLevelsIndices.GetMaxAvailableLevelIndex() && this.featureManager.CanActivateFeature(this.featureHandler);
        }

        public bool HasActivationTriggerForLevel(int unlockedLevelIndex)
        {
            if (!this.FeatureEnabled())
            {
                return false;
            }
            if (this.HasActiveFeature())
            {
                return false;
            }
            if (this.Config.UseTriggerLevels)
            {
                foreach (int num in this.Config.TriggerLevels)
                {
                    int humanNumberFromLevelIndex = this.mainLevelsIndices.GetHumanNumberFromLevelIndex(unlockedLevelIndex);
                    if (humanNumberFromLevelIndex == num)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private void ActivateLevelRush(FeatureData featureData)
        {
            ActivatedFeatureInstanceData initialLevelRushValues = this.featureManager.ActivateFeature(this.featureHandler, featureData);
            this.SetInitialLevelRushValues(initialLevelRushValues);
        }

        private void SetInitialLevelRushValues(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            LevelRushInstanceCustomData customInstanceData = activatedFeatureInstanceData.GetCustomInstanceData<LevelRushInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this.featureHandler);
            customInstanceData.StartLevel = this.mainProgression.GetFarthestUnlockedLevelIndex();
            customInstanceData.Progress = 0;
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public int GetSecondsLeft()
        {
            return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this.featureHandler);
        }

        private LevelRushInstanceCustomData InstanceData
        {
            get
            {
                return this.featureManager.GetActivatedFeature(this.featureHandler).GetCustomInstanceData<LevelRushInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this.featureHandler);
            }
        }

        private LevelRushConfig Config
        {
            get
            {
                return this.configGetter.Get();
            }
        }

        private readonly ILevelRushFeatureHandler featureHandler;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly IMainLevelsIndices mainLevelsIndices;

        private readonly IMainProgression mainProgression;

        private readonly IConfigGetter<LevelRushConfig> configGetter;
    }
}
