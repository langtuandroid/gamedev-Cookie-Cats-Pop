using System;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGame.MainLevels
{
    public class MainLevelDatabase : LevelDatabase
    {
        public int WeekOneMaxAvailableHumanLevel
        {
            get
            {
                return this.weekOneMaxAvailableHumanLevel;
            }
        }

        public int MaxSupportedHumanLevel
        {
            get
            {
                return this.maxSupportedHumanLevel;
            }
        }

        public override LevelProxy GetLevel(int levelIndex)
        {
            if (levelIndex < 0)
            {
                return LevelProxy.Invalid;
            }
            return new LevelProxy(this, new int[]
            {
                levelIndex
            });
        }

        public override string GetAnalyticsDescriptor()
        {
            return "main";
        }

        public override MapIdentifier GetMapAndLevelsIdentifier()
        {
            return "Main";
        }

        public override string GetPersistedKey(LevelProxy levelProxy)
        {
            return MainProgressionManager.Instance.GetPersistedKey(levelProxy);
        }

        public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
        {
            return MainProgressionManager.Instance.GetLevelData(createIfNotExisting, levelProxy);
        }

        public override bool ValidateLevelIndex(int index)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                return base.ValidateLevelIndex(index);
            }
            return base.ValidateLevelIndex(index) && index <= MainProgressionManager.Instance.MaxAvailableLevel;
        }

        public override double GetGateProgress(LevelProxy levelProxy)
        {
            return MainProgressionManager.Instance.GetGateProgress(levelProxy);
        }

        public override void RemoveLevelData(LevelProxy levelProxy)
        {
            MainProgressionManager.Instance.RemoveLevelData(levelProxy);
        }

        public override void Save()
        {
            MainProgressionManager.Instance.Save();
        }

        public override LevelDifficulty GetLevelDifficulty(int levelId)
        {
            HardLevelsManager featureHandler = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<HardLevelsManager>();
            LevelProxy level = this.GetLevel(levelId);
            if (!(this.GetLevelMetaData(levelId) is GateMetaData) && featureHandler.IsLevelHard(level))
            {
                return LevelDifficulty.Hard;
            }
            if (levelId < base.LevelDifficultyList.Count)
            {
                return base.LevelDifficultyList[levelId];
            }
            return LevelDifficulty.Normal;
        }

        public const int NO_LEVEL = -1;

        [SerializeField]
        private int weekOneMaxAvailableHumanLevel;

        [SerializeField]
        private int maxSupportedHumanLevel;
    }
}
