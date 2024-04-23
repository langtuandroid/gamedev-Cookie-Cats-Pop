using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public sealed class TreasureHuntManager : IFeatureTypeHandler<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
    {
        public TreasureHuntManager(TactileModules.FeatureManager.FeatureManager featureManager, ITreasureHuntProvider provider, IPlayFlowFactory playFlowFactory, IFlowStack flowStack, IFullScreenManager fullScreenManager, MapFacade mapFacade)
        {
            this.featureManager = featureManager;
            this.playFlowFactory = playFlowFactory;
            this.flowStack = flowStack;
            this.fullScreenManager = fullScreenManager;
            this.mapFacade = mapFacade;
            this.provider = provider;
        }

        public ITreasureHuntProvider provider { get; private set; }

        public int SecondsLeft
        {
            get
            {
                return this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this);
            }
        }

        public TreasureHuntInstanceCustomData CustomData
        {
            get
            {
                return this.GetFeatureInstanceCustomData<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>();
            }
        }

        public TreasureHuntConfig Config
        {
            get
            {
                return this.provider.Config();
            }
        }

        public TreasureHuntLevelDatabase TreasureHuntLevelDatabase
        {
            get
            {
                return ManagerRepository.Get<LevelDatabaseCollection>().GetLevelDatabase<TreasureHuntLevelDatabase>("TreasureHunt");
            }
        }

        public bool IsActive
        {
            get
            {
                return this.featureManager.HasActiveFeature(this) && !this.CustomData.rewardClaimed;
            }
        }

        public string TimeLeftAsText
        {
            get
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, this.SecondsLeft);
                int num = (int)timeSpan.TotalSeconds;
                if (num <= 0)
                {
                    return L.Get((this.LevelsLeft != 1) ? "Ended" : "Last chance!");
                }
                return L.FormatSecondsAsColumnSeparated(num, "Ended", TimeFormatOptions.None);
            }
        }

        public int FarthestCompletedLevel
        {
            get
            {
                return (this.CustomData != null) ? this.CustomData.farthestCompletedLevel : -1;
            }
        }

        public LevelProxy GetNextLevel
        {
            get
            {
                return this.TreasureHuntLevelDatabase.GetLevel(this.FarthestCompletedLevel + 1);
            }
        }

        public void LevelCompleted(LevelProxy levelProxy)
        {
            this.CustomData.farthestCompletedLevel = levelProxy.Index;
            this.Save();
        }

        public void Save()
        {
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public IEnumerator ClaimReward()
        {
            yield return new Fiber.OnExit(delegate ()
            {
                this.featureManager.DeactivateFeature(this);
            });
            this.CustomData.rewardClaimed = true;
            yield return this.AddRewardToInventoryAndShowRewardView(this.provider.Config().Rewards);
            yield break;
        }

        private int LevelsLeft
        {
            get
            {
                return this.LevelCount - 1 - (this.FarthestCompletedLevel + 1);
            }
        }

        private int LevelCount
        {
            get
            {
                LevelProxy levelProxy = new LevelProxy(this.TreasureHuntLevelDatabase, new int[1]);
                return (levelProxy.LevelMetaData as GateMetaData).levelCount;
            }
        }

        public IEnumerator ShowEventStartView(bool startSessionPopup)
        {
            UIViewManager.UIViewStateGeneric<TreasureHuntRewardInfoView> vs = UIViewManager.Instance.ShowView<TreasureHuntRewardInfoView>(new object[0]);
            yield return vs.WaitForClose();
            yield break;
        }

        public IEnumerator ShowEventEndedView()
        {
            UIViewManager.UIViewStateGeneric<TreasureHuntEndedView> vs = UIViewManager.Instance.ShowView<TreasureHuntEndedView>(new object[0]);
            yield return vs.WaitForClose();
            yield break;
        }

        public void SwitchToTreasureHuntMapView()
        {
            this.flowStack.Push(new TreasureHuntMapFlow("TreasureHunt", this.mapFacade, this.fullScreenManager, this.flowStack, this, this.playFlowFactory));
        }

        private IEnumerator AddRewardToInventoryAndShowRewardView(List<ItemAmount> rewards)
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                InventoryManager.Instance.Add(rewards[i].ItemId, rewards[i].Amount, "TreasureHuntReward");
            }
            UIViewManager.UIViewStateGeneric<TreasureHuntRewardView> vs = UIViewManager.Instance.ShowView<TreasureHuntRewardView>(new object[0]);
            yield return vs.WaitForClose();
            yield break;
        }

        public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
        {
            activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
            this.Save();
        }

        public void FadeToBlack()
        {
        }

        public string FeatureType
        {
            get
            {
                return "treasure-hunt";
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
                return JsonSerializer.ObjectToHashtable(new TreasureHuntMetaData
                {
                    levelSetOverrideIndex = 0
                });
            }
            throw new NotSupportedException();
        }

        public TreasureHuntInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
        {
            return new TreasureHuntInstanceCustomData();
        }

        public FeatureTypeCustomData NewFeatureTypeCustomData()
        {
            return new FeatureTypeCustomData();
        }

        public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : TreasureHuntInstanceCustomData
        {
            toMerge.farthestCompletedLevel = Mathf.Max(current.farthestCompletedLevel, cloud.farthestCompletedLevel);
            toMerge.rewardClaimed = (current.rewardClaimed || cloud.rewardClaimed);
        }

        public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
        {
        }

        public FeatureNotificationSettings FeatureNotificationSettings
        {
            get
            {
                return this.Config.FeatureNotificationSettings;
            }
        }

        public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
        {
            return this.provider.GetNotificationText(timeSpan, instanceData);
        }

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly IPlayFlowFactory playFlowFactory;

        private readonly IFlowStack flowStack;

        private readonly IFullScreenManager fullScreenManager;

        private readonly MapFacade mapFacade;
    }
}
