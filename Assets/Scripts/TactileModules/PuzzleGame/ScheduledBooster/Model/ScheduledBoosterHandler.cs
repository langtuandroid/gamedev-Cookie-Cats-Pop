using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
    [MainMapFeature]
    public class ScheduledBoosterHandler : IFeatureTypeHandler<ScheduledBoosterInstanceCustomData, ScheduledBoosterMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
    {
        public ScheduledBoosterHandler(TactileModules.FeatureManager.FeatureManager featureManager, ConfigurationManager configurationManager, ScheduledBoosters scheduledBoosters, IScheduledBoosterFactory scheduledBoosterFactory)
        {
            this.featureManager = featureManager;
            this.configurationManager = configurationManager;
            this.scheduledBoosters = scheduledBoosters;
            this.scheduledBoosterFactory = scheduledBoosterFactory;
        }

        public string FeatureType
        {
            get
            {
                return "limited-availability-booster";
            }
        }

        public bool AllowMultipleFeatureInstances
        {
            get
            {
                return true;
            }
        }

        public int MetaDataVersion
        {
            get
            {
                return 2;
            }
        }

        public ScheduledBoosterConfig Config
        {
            get
            {
                return this.configurationManager.GetConfig<ScheduledBoosterConfig>();
            }
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.Config.FeatureNotificationSettings;
            }
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.SetEndUnixTime(1);
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public void FadeToBlack()
        {
            if (!this.boostersConstructed)
            {
                this.boostersConstructed = true;
                this.ConstructBoosters();
            }
            this.ActivateFeaturesIfPossible();
            this.DeactivateInactiveFeaturesIfPossible();
        }

        private void ConstructBoosters()
        {
            List<ActivatedFeatureInstanceData> activatedFeatures = this.featureManager.GetActivatedFeatures(this);
            if (activatedFeatures == null)
            {
                return;
            }
            foreach (ActivatedFeatureInstanceData featureInstanceData in activatedFeatures)
            {
                IScheduledBooster booster = this.scheduledBoosterFactory.Create(featureInstanceData);
                this.scheduledBoosters.AddBooster(booster);
            }
        }

        private void ActivateFeaturesIfPossible()
        {
            foreach (FeatureData featureData in this.featureManager.GetAvailableFeatures(this))
            {
                if (this.featureManager.CanActivateFeature(this, featureData))
                {
                    ActivatedFeatureInstanceData featureInstanceData = this.featureManager.ActivateFeature(this, featureData);
                    IScheduledBooster booster = this.scheduledBoosterFactory.Create(featureInstanceData);
                    this.scheduledBoosters.AddBooster(booster);
                }
            }
        }

        private void DeactivateInactiveFeaturesIfPossible()
        {
            List<IScheduledBooster> list = new List<IScheduledBooster>();
            List<IScheduledBooster> boosters = this.scheduledBoosters.GetBoosters();
            foreach (IScheduledBooster scheduledBooster in boosters)
            {
                if (this.ShouldDeactivateFeature(scheduledBooster))
                {
                    list.Add(scheduledBooster);
                }
            }
            foreach (IScheduledBooster scheduledBooster2 in list)
            {
                this.scheduledBoosters.RemoveBooster(scheduledBooster2.Type);
                this.featureManager.DeactivateFeature(this, scheduledBooster2.FeatureInstanceData);
            }
        }

        private bool ShouldDeactivateFeature(IScheduledBooster booster)
        {
            return this.featureManager.ShouldDeactivateFeature(this, booster.FeatureInstanceData) && !booster.IsActive;
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            if (fromVersion == 0 && toVersion == 1)
            {
                return metaData;
            }
            if (fromVersion == 1 && toVersion == 2)
            {
                return metaData;
            }
            throw new NotSupportedException();
        }

        public ScheduledBoosterInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new ScheduledBoosterInstanceCustomData();
        }

        public FeatureTypeCustomData NewFeatureTypeCustomData()
        {
            return new FeatureTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : ScheduledBoosterInstanceCustomData
        {
            toMerge.NumberOfBoostersUsed = Mathf.Max(current.NumberOfBoostersUsed, cloud.NumberOfBoostersUsed);
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
        {
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            string arg = instanceData.GetMetaData<ScheduledBoosterMetaData>().ScheduledBoosterType.ToString();
            return string.Format(L.Get("There is only {0} hours left to buy the limited booster: {1}!"), timeSpan.TotalHours, arg);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly ConfigurationManager configurationManager;

        private readonly ScheduledBoosters scheduledBoosters;

        private readonly IScheduledBoosterFactory scheduledBoosterFactory;

        private bool boostersConstructed;
    }
}
