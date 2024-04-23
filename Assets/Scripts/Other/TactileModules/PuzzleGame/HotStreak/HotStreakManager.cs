using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.HotStreak.Analytics;
using TactileModules.PuzzleGame.HotStreak.Data;
using TactileModules.PuzzleGame.HotStreak.Popups;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

//namespace TactileModules.PuzzleGame.HotStreak
//{
    [MainMapFeature]
    public class HotStreakManager : IFeatureTypeHandler<HotStreakInstanceCustomData, HotStreakMetaData, HotStreakTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
    {
        public HotStreakManager(TactileModules.FeatureManager.FeatureManager featureManager, IHotStreakProvider provider, IPlayFlowEvents playFlowEvents)
        {
            playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
            this.featureManager = featureManager;
            this.Provider = provider;
            new HotStreakStartPopup(this, featureManager);
            new HotStreakProgressPopup(this, featureManager);
            new HotStreakSessionPopup(this, featureManager);
        }

        public IHotStreakProvider Provider { get; private set; }

        private void HandlePlayFlowCreated(ICorePlayFlow playFlow)
        {
            playFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.LevelPlayed));
        }

        private IEnumerator LevelPlayed(ILevelAttempt levelAttempt)
        {
            LevelProxy levelProxy = levelAttempt.LevelProxy;
            if (!(levelProxy.RootDatabase is MainLevelDatabase))
            {
                yield break;
            }
            if (levelAttempt.WasCompletedForTheFirstTime)
            {
                this.IncreaseProgress(levelProxy.HumanNumber);
                yield break;
            }
            if (levelAttempt.DidPlayAndFail)
            {
                this.ResetProgress(levelProxy.HumanNumber);
                yield break;
            }
            yield break;
        }

        public bool ProgressChanged { get; private set; }

        public int PreviousProgress { get; private set; }

        public void IncreaseProgress(int levelNumber)
        {
            if (!this.IsHotStreakActive)
            {
                return;
            }
            this.Progress++;
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            HotStreakAnalytics.LogStreakTierReached(activatedFeature.Id, levelNumber, this.CurrentTierIndex + 1);
        }

        public void ResetProgress(int levelNumber)
        {
            if (!this.IsHotStreakActive)
            {
                return;
            }
            int progress = this.Progress;
            int tierReached = this.CurrentTierIndex + 1;
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            HotStreakAnalytics.LogStreakLost(activatedFeature.Id, levelNumber, progress, tierReached);
            this.Progress = 0;
        }

        public int Progress
        {
            get
            {
                return (!this.IsHotStreakActive) ? 0 : this.InstanceCustomData.Progress;
            }
            private set
            {
                if (!this.IsHotStreakActive)
                {
                    return;
                }
                this.PreviousProgress = this.InstanceCustomData.Progress;
                this.InstanceCustomData.Progress = value;
                if (this.PreviousProgress != value && value <= this.MaxWins)
                {
                    this.ProgressChanged = true;
                }
            }
        }

        private HotStreakInstanceCustomData InstanceCustomData
        {
            get
            {
                return this.GetFeatureInstanceCustomData<HotStreakInstanceCustomData, HotStreakMetaData, HotStreakTypeCustomData>();
            }
        }

        private HotStreakTypeCustomData TypeCustomData
        {
            get
            {
                return this.GetFeatureTypeCustomData<HotStreakInstanceCustomData, HotStreakMetaData, HotStreakTypeCustomData>();
            }
        }

        public int CurrentTierIndex
        {
            get
            {
                int result = -1;
                List<HotStreakTier> tiers = this.GetTiers();
                for (int i = tiers.Count - 1; i >= 0; i--)
                {
                    if (this.Progress >= tiers[i].RequiredWins)
                    {
                        result = i;
                        break;
                    }
                }
                return result;
            }
        }

        public HotStreakTier CurrentTier
        {
            get
            {
                return (this.CurrentTierIndex < 0) ? this.TIER_ZERO : this.GetTiers()[this.CurrentTierIndex];
            }
        }

        public List<HotStreakTier> GetTiers()
        {
            if (this.IsLocalFeature)
            {
                return this.Provider.Config.Tiers;
            }
            ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
            if (activatedFeature == null)
            {
                return new List<HotStreakTier>();
            }
            HotStreakMetaData metaData = activatedFeature.GetMetaData<HotStreakInstanceCustomData, HotStreakMetaData, HotStreakTypeCustomData>(this);
            return metaData.Tiers;
        }

        public List<ItemAmount> CurrentTierBonus
        {
            get
            {
                return this.CurrentTier.Bonuses;
            }
        }

        public List<ItemAmount> GetBonusForTier(int tierIndex)
        {
            if (tierIndex < 0 || tierIndex > this.GetTiers().Count - 1)
            {
                return new List<ItemAmount>();
            }
            return this.GetTiers()[tierIndex].Bonuses;
        }

        public int MaxWins
        {
            get
            {
                List<HotStreakTier> tiers = this.GetTiers();
                return (tiers.Count <= 0) ? 0 : tiers[tiers.Count - 1].RequiredWins;
            }
        }

        public bool ShouldActivateHotStreak()
        {
            return this.Provider.Config.FeatureEnabled && this.TypeCustomData.HasEverStartedLocalFeature && PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber >= this.Provider.Config.LevelRequiredGlobal && this.featureManager.CanActivateFeature(this);
        }

        public bool ShouldActiveLocalHotStreak()
        {
            return this.Provider.Config.FeatureEnabled && !this.TypeCustomData.HasEverStartedLocalFeature && PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber >= this.Provider.Config.LevelRequiredLocal && this.featureManager.CanActivateLocalFeature(this);
        }

        public bool ShouldDeactivateHotStreak()
        {
            return this.featureManager.ShouldDeactivateFeature(this);
        }

        public void DeactivateHotStreak()
        {
            this.featureManager.DeactivateFeature(this);
        }

        public bool IsHotStreakActive
        {
            get
            {
                return this.Provider.Config.FeatureEnabled && this.HasActiveFeature();
            }
        }

        public bool IsLocalFeature
        {
            get
            {
                ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
                return activatedFeature != null && activatedFeature.FeatureData.MetaData.ContainsKey("isLocal") && (bool)activatedFeature.FeatureData.MetaData["isLocal"];
            }
        }

        public int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public string GetTimeRemainingForHotStreakAsString()
        {
            return L.FormatSecondsAsColumnSeparated(this.SecondsLeft, L.Get("Ended"), TimeFormatOptions.None);
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public IEnumerator ShowProgressViewAndWait()
        {
            yield return this.Provider.ShowProgressViewAndWait();
            this.ProgressChanged = false;
            yield break;
        }

        public void ActivateHotStreak(bool local)
        {
            if (local)
            {
                this.featureManager.ActivateLocalFeature(this, this.Provider.Config.MaxPlayTimeDurationLocal);
                this.TypeCustomData.HasEverStartedLocalFeature = true;
            }
            else
            {
                this.featureManager.ActivateFeature(this);
            }
        }

        public void FadeToBlack()
        {
            if (this.ShouldDeactivateHotStreak())
            {
                this.DeactivateHotStreak();
            }
        }

        public string FeatureType
        {
            get
            {
                return "hot-streak";
            }
        }

        public bool AllowMultipleFeatureInstances
        {
            get
            {
                return false;
            }
        }

        public int MetaDataVersion
        {
            get
            {
                return 1;
            }
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public HotStreakInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new HotStreakInstanceCustomData();
        }

        public HotStreakTypeCustomData NewFeatureTypeCustomData()
        {
            return new HotStreakTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : HotStreakInstanceCustomData
        {
            toMerge.Progress = Mathf.Max(current.Progress, cloud.Progress);
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : HotStreakTypeCustomData
        {
            toMerge.HasEverStartedLocalFeature = (current.HasEverStartedLocalFeature || cloud.HasEverStartedLocalFeature);
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.Provider.Config.FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return this.Provider.GetTextForNotification(timeSpan, instanceData);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        public readonly HotStreakTier TIER_ZERO = new HotStreakTier();
    }
//}
