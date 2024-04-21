using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntLevelDatabase : LevelDatabase
    {
        private static TreasureHuntManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>();
            }
        }

        public override LevelProxy GetLevel(int levelIndex)
        {
            int num = 0;
            int num2 = -1;
            TreasureHuntManager manager = TreasureHuntLevelDatabase.Manager;
            if (manager.HasActiveFeature())
            {
                ActivatedFeatureInstanceData activatedFeature = TreasureHuntLevelDatabase.Manager.GetActivatedFeature();
                TreasureHuntMetaData metaData = activatedFeature.GetMetaData<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>(manager);
                num = Mathf.Abs(activatedFeature.Id.GetHashCode());
                int levelSetOverrideIndex = metaData.levelSetOverrideIndex;
                num2 = levelSetOverrideIndex - 1;
            }
            if (num2 < 0 || num2 >= this.NumberOfAvailableLevels)
            {
                num2 = num % this.NumberOfAvailableLevels;
            }
            LevelProxy levelProxy = new LevelProxy(this, new int[]
            {
                num2
            });
            return levelProxy.CreateChildProxy(levelIndex);
        }

        public override string GetAnalyticsDescriptor()
        {
            return "tressureHunt";
        }

        public override MapIdentifier GetMapAndLevelsIdentifier()
        {
            return "TreasureHunt";
        }

        public override string GetPersistedKey(LevelProxy levelProxy)
        {
            return "DummyEntryTressureHunt";
        }

        public override void Save()
        {
            TreasureHuntLevelDatabase.Manager.Save();
        }

        public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
        {
            int index = levelProxy.Index;
            int farthestCompletedLevel = TreasureHuntLevelDatabase.Manager.FarthestCompletedLevel;
            ILevelAccomplishment levelAccomplishment = TreasureHuntLevelDatabase.Manager.provider.NewLevelAccomplishment();
            if (index <= farthestCompletedLevel)
            {
                levelAccomplishment.Points = 1;
            }
            return levelAccomplishment;
        }

        public override void RemoveLevelData(LevelProxy levelProxy)
        {
        }

        public override double GetGateProgress(LevelProxy levelProxy)
        {
            return 0.0;
        }

        public override int GetHumanNumber(LevelProxy levelProxy)
        {
            return levelProxy.LevelCollection.LevelStubs[levelProxy.Index].humanNumber;
        }
    }
}
