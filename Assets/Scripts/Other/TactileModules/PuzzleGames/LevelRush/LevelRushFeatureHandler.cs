using System;
using System.Collections;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
    [MainMapFeature]
    public sealed class LevelRushFeatureHandler : ILevelRushFeatureHandler, IFeatureTypeHandler<LevelRushInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
    {
        public LevelRushFeatureHandler(TactileModules.FeatureManager.FeatureManager featureManager, IConfigGetter<LevelRushConfig> configGetter, ILevelRushNotificationProvider provider)
        {
            featureManager.AddMergeDependency(typeof(MainProgressionManager.PersistableState));
            this.configGetter = configGetter;
            this.provider = provider;
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public void FadeToBlack()
        {
        }

        public string FeatureType
        {
            get
            {
                return "level-rush";
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
                return 0;
            }
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public LevelRushInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new LevelRushInstanceCustomData();
        }

        public FeatureTypeCustomData NewFeatureTypeCustomData()
        {
            return new FeatureTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : LevelRushInstanceCustomData
        {
            toMerge.LatestRewardClaimed = Mathf.Max(current.LatestRewardClaimed, cloud.LatestRewardClaimed);
            toMerge.Progress = Mathf.Max(current.Progress, cloud.Progress);
            toMerge.StartLevel = PuzzleGameData.PlayerState.FarthestUnlockedLevelIndex - toMerge.Progress;
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
        {
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.configGetter.Get().FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return this.provider.GetTextForNotification(timeSpan, instanceData);
        }

        public LevelRushInstanceCustomData CustomData
        {
            get
            {
                return this.GetFeatureInstanceCustomData<LevelRushInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>();
            }
        }

        public const string FEATURE_TYPE_ID = "level-rush";

        private readonly ConfigurationManager configurationManager;

        private readonly IConfigGetter<LevelRushConfig> configGetter;

        private readonly ILevelRushNotificationProvider provider;
    }
}
