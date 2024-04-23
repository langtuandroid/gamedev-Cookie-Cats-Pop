using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
    [MainMapFeature]
    public sealed class StoryMapEventFeatureHandler : IStoryMapEventFeatureHandler, IFeatureTypeHandler<FeatureInstanceCustomData, StoryMapEventMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureAssetBundleHandler, IFeatureTypeHandler
    {
        public StoryMapEventFeatureHandler(IConfigGetter<StoryMapEventConfig> configGetter, IStoryMapEventNotificationProvider provider)
        {
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
                return "story-map";
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
                return 2;
            }
        }

        public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
        {
            if (fromVersion == 0 && toVersion == 1)
            {
                return metaData;
            }
            if (fromVersion == 1 && toVersion == 2)
            {
                StoryMapEventMetaData storyMapEventMetaData = new StoryMapEventMetaData();
                string key = "AssetBundleName";
                if (metaData.ContainsKey(key))
                {
                    storyMapEventMetaData.AssetBundleNames.Add((string)metaData[key]);
                }
                return JsonSerializer.ObjectToHashtable(storyMapEventMetaData);
            }
            throw new NotSupportedException();
        }

        public FeatureInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new FeatureInstanceCustomData();
        }

        public FeatureTypeCustomData NewFeatureTypeCustomData()
        {
            return new FeatureTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : FeatureInstanceCustomData
        {
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

        public List<string> GetAssetBundles(FeatureData featureData)
        {
            List<string> list = new List<string>();
            StoryMapEventMetaData metaData = featureData.GetMetaData(this);
            foreach (string text in metaData.AssetBundleNames)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    list.Add(text);
                }
            }
            return list;
        }

        public void FeatureInstanceWasHidden(ActivatedFeatureInstanceData instanceData)
        {
        }

        public const string FEATURE_TYPE_ID = "story-map";

        private readonly IConfigGetter<StoryMapEventConfig> configGetter;

        private readonly IStoryMapEventNotificationProvider provider;

        private bool isPlayingStoryMap;
    }
}
