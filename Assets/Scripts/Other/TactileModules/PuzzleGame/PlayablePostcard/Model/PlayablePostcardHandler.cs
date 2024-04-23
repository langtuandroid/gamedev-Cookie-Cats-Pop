using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PlayablePostcard.Data;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
    public class PlayablePostcardHandler : MapFeatureHandler<PlayablePostcardInstanceCustomData, PlayablePostcardMetaData, PlayablePostcardTypeCustomData>, IFeatureNotifications
    {
        public PlayablePostcardHandler(TactileModules.FeatureManager.FeatureManager featureManager, PlayablePostcardMapFeatureProvider mapFeatureProvider, PlayablePostcardConfig config, IMainProgression mainProgressionManager)
        {
            this.featureManager = featureManager;
            this.mapFeatureProvider = mapFeatureProvider;
            this.config = config;
            this.mainProgressionManager = mainProgressionManager;
        }

        public override string FeatureType
        {
            get
            {
                return "playable-postcard";
            }
        }

        public override bool AllowMultipleFeatureInstances
        {
            get
            {
                return false;
            }
        }

        public override int MetaDataVersion
        {
            get
            {
                return 1;
            }
        }

        protected override IMapFeatureProvider MapFeatureProvider
        {
            get
            {
                return this.mapFeatureProvider;
            }
        }

        public override void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public override void FadeToBlack()
        {
            if (this.featureManager.CanActivateFeature(this) && this.config.LevelRequired <= this.mainProgressionManager.GetFarthestCompletedLevelIndex())
            {
                this.featureManager.ActivateFeature(this);
            }
        }

        public override Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public override PlayablePostcardInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new PlayablePostcardInstanceCustomData();
        }

        public override PlayablePostcardTypeCustomData NewFeatureTypeCustomData()
        {
            return new PlayablePostcardTypeCustomData();
        }

        protected override void MergeFeatureInstanceStatesImplementor<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud)
        {
            if (current.Postcard.Count != cloud.Postcard.Count)
            {
                if (current.Postcard.Count > cloud.Postcard.Count)
                {
                    PlayablePostcardInstanceCustomData.TakeState<FeatureState>(ref toMerge, current);
                }
                else
                {
                    PlayablePostcardInstanceCustomData.TakeState<FeatureState>(ref toMerge, cloud);
                }
            }
        }

        protected override void MergeFeatureTypeStateImplementor<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud)
        {
        }

        public override string GetTimeLeftAsText()
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, this.SecondsLeft);
            int totalSeconds = (int)timeSpan.TotalSeconds;
            return L.FormatSecondsAsColumnSeparated(totalSeconds, "Ended", TimeFormatOptions.None);
        }

        public int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.config.FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return string.Format(L.Get("There is only {0} hours left to play playable postcards"), timeSpan.TotalHours);
        }

        public PlayablePostcardInstanceCustomData InstanceCustomData
        {
            get
            {
                return this.GetFeatureInstanceCustomData<PlayablePostcardInstanceCustomData, PlayablePostcardMetaData, PlayablePostcardTypeCustomData>();
            }
        }

        public PlayablePostcardMetaData MetaData
        {
            get
            {
                return this.GetFeatureInstanceMetaData<PlayablePostcardInstanceCustomData, PlayablePostcardMetaData, PlayablePostcardTypeCustomData>();
            }
        }

        public bool IsActive()
        {
            return this.featureManager.IsFeatureActivated(this);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly IMapFeatureProvider mapFeatureProvider;

        private readonly PlayablePostcardConfig config;

        private readonly IMainProgression mainProgressionManager;
    }
}
