using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
    public class SlidesAndLaddersHandler : MapFeatureHandler<SlidesAndLaddersInstanceCustomData, SlidesAndLaddersMetaData, SlidesAndLaddersTypeCustomData>, IFeatureNotifications, ISlidesAndLaddersHandler
    {
        public SlidesAndLaddersHandler(TactileModules.FeatureManager.FeatureManager featureManager, MapStreamerCollection mapStreamerCollection, ISlidesAndLaddersMapViewProvider mapViewProvider, ISlidesAndLaddersLevelDatabase levelDatabase, ISlidesAndLaddersSave save, ConfigProvider<SlidesAndLaddersConfig> config)
        {
            this.featureManager = featureManager;
            this.levelDatabase = levelDatabase;
            this.config = config;
            this.save = save;
            this.MapViewProvider = mapViewProvider;
            this.MapStreamerCollection = mapStreamerCollection;
        }

        public override string FeatureType
        {
            get
            {
                return "slides-and-ladders";
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

        public ISlidesAndLaddersLevelDatabase LevelDatabase
        {
            get
            {
                return this.levelDatabase;
            }
        }

        protected override IMapFeatureProvider MapFeatureProvider
        {
            get
            {
                if (this.mapFeatureProvider == null)
                {
                    this.mapFeatureProvider = new SlidesAndLaddersMapFeatureProvider(ManagerRepository.Get<SlidesAndLaddersSystem>().GetControllerFactory());
                }
                return this.mapFeatureProvider;
            }
        }

        public int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public SlidesAndLaddersInstanceCustomData InstanceCustomData
        {
            get
            {
                return this.GetFeatureInstanceCustomData<SlidesAndLaddersInstanceCustomData, SlidesAndLaddersMetaData, SlidesAndLaddersTypeCustomData>();
            }
        }

        public SlidesAndLaddersMetaData MetaData
        {
            get
            {
                return this.GetFeatureInstanceMetaData<SlidesAndLaddersInstanceCustomData, SlidesAndLaddersMetaData, SlidesAndLaddersTypeCustomData>();
            }
        }

        public MapStreamerCollection MapStreamerCollection { get; private set; }

        public ISlidesAndLaddersMapViewProvider MapViewProvider { get; private set; }

        public override void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public override void FadeToBlack()
        {
            if (this.InstanceCustomData != null && this.InstanceCustomData.CompletedFeature)
            {
                TactileModules.FeatureManager.FeatureManager.Instance.DeactivateFeature(this);
            }
        }

        public override Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            throw new NotSupportedException();
        }

        public override SlidesAndLaddersInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new SlidesAndLaddersInstanceCustomData();
        }

        public override SlidesAndLaddersTypeCustomData NewFeatureTypeCustomData()
        {
            return new SlidesAndLaddersTypeCustomData();
        }

        protected override void MergeFeatureInstanceStatesImplementor<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud)
        {
            if (current.RewardsClaimed.Count < cloud.RewardsClaimed.Count)
            {
                SlidesAndLaddersInstanceCustomData.TakeState<FeatureState>(ref toMerge, cloud);
            }
            else if (current.RewardsClaimed.Count == cloud.RewardsClaimed.Count && current.FarthestUnlockedLevelIndex < cloud.FarthestUnlockedLevelIndex)
            {
                SlidesAndLaddersInstanceCustomData.TakeState<FeatureState>(ref toMerge, cloud);
            }
        }

        protected override void MergeFeatureTypeStateImplementor<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud)
        {
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.config.Get().FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return string.Format(L.Get("There is only {0} hours left to play slides and ladders"), timeSpan.TotalHours);
        }

        public override string GetTimeLeftAsText()
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, this.SecondsLeft);
            int totalSeconds = (int)timeSpan.TotalSeconds;
            return L.FormatSecondsAsColumnSeparated(totalSeconds, "Ended", TimeFormatOptions.None);
        }

        public List<ItemAmount> GetFeatureRewards()
        {
            List<ItemAmount> list = new List<ItemAmount>();
            list.AddRange(this.InstanceCustomData.AddedChestRewards);
            list.AddRange(this.MetaData.Rewards);
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (ItemAmount itemAmount in list)
            {
                if (dictionary.ContainsKey(itemAmount.ItemId))
                {
                    Dictionary<string, int> dictionary2;
                    string itemId;
                    (dictionary2 = dictionary)[itemId = itemAmount.ItemId] = dictionary2[itemId] + itemAmount.Amount;
                }
                else
                {
                    dictionary.Add(itemAmount.ItemId, itemAmount.Amount);
                }
            }
            List<ItemAmount> list2 = new List<ItemAmount>();
            foreach (KeyValuePair<string, int> keyValuePair in dictionary)
            {
                list2.Add(new ItemAmount
                {
                    ItemId = keyValuePair.Key,
                    Amount = keyValuePair.Value
                });
            }
            return list2;
        }

        public T GetDataInstance<T>()
        {
            if (this.InstanceCustomData.GetType() == typeof(T))
            {
                return (T)((object)this.InstanceCustomData);
            }
            if (this.MetaData.GetType() == typeof(T))
            {
                return (T)((object)this.MetaData);
            }
            return default(T);
        }

        public void Save()
        {
            this.save.Save();
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly ISlidesAndLaddersLevelDatabase levelDatabase;

        private readonly ISlidesAndLaddersSave save;

        private readonly ConfigProvider<SlidesAndLaddersConfig> config;

        private IMapFeatureProvider mapFeatureProvider;

        private Dictionary<string, Type> featureClasses = new Dictionary<string, Type>();
    }
}
