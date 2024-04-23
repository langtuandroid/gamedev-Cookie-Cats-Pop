using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.LevelRush
{
    public class LevelRushProgression : ILevelRushProgression
    {
        public LevelRushProgression(ILevelRushFeatureHandler levelRushFeatureHandler, InventoryManager inventoryManager, IMainLevelsIndices mainLevelsIndices, IConfigGetter<LevelRushConfig> configGetter, IMainProgression mainProgression)
        {
            this.levelRushFeatureHandler = levelRushFeatureHandler;
            this.inventoryManager = inventoryManager;
            this.mainLevelsIndices = mainLevelsIndices;
            this.configGetter = configGetter;
            this.mainProgression = mainProgression;
        }

        public void AddRewardToInventory(LevelRushConfig.Reward reward)
        {
            for (int i = 0; i < reward.Items.Count; i++)
            {
                ItemAmount itemAmount = reward.Items[i];
                this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "LevelRushReward");
            }
        }

        public int GetLevelIndexFromRewardIndex(int rewardIndex)
        {
            if (rewardIndex >= this.Config.LevelRushRewards.Count)
            {
                return int.MaxValue;
            }
            return this.Config.LevelRushRewards[rewardIndex].LevelIndex + this.StartLevel;
        }

        public LevelRushConfig.Reward GetReward(int index)
        {
            return this.Config.LevelRushRewards[index];
        }

        public int GetRewardIndex(LevelRushConfig.Reward reward)
        {
            return this.Config.LevelRushRewards.IndexOf(reward);
        }

        public bool IsRewardBeyondFarthestUnlockedLevel(int rewardIndex)
        {
            return this.GetLevelIndexFromRewardIndex(rewardIndex) > this.mainProgression.GetFarthestUnlockedLevelIndex();
        }

        public List<LevelRushConfig.Reward> GetUnclaimedRewards()
        {
            List<LevelRushConfig.Reward> list = new List<LevelRushConfig.Reward>();
            if (this.levelRushFeatureHandler.HasActiveFeature())
            {
                foreach (LevelRushConfig.Reward reward in this.Config.LevelRushRewards)
                {
                    if (!this.IsRewardClaimed(reward))
                    {
                        if (this.GetLevelIndexFromReward(reward) < this.LevelCount)
                        {
                            list.Add(reward);
                        }
                    }
                }
            }
            return list;
        }

        public LevelRushConfig.Reward ClaimNextUnclaimedReward()
        {
            int num = this.LatestRewardClaimed + 1;
            if (num >= this.Config.LevelRushRewards.Count)
            {
                return null;
            }
            this.LatestRewardClaimed = num;
            PuzzleGameData.UserSettings.SaveLocal();
            return this.Config.LevelRushRewards[num];
        }

        public bool IsRewardOnNextLevelToComplete(int rewardIndex)
        {
            int levelIndexFromRewardIndex = this.GetLevelIndexFromRewardIndex(rewardIndex);
            int farthestUnlockedLevelIndex = this.mainProgression.GetFarthestUnlockedLevelIndex();
            return levelIndexFromRewardIndex - farthestUnlockedLevelIndex == 1;
        }

        public bool IsLastReward(LevelRushConfig.Reward reward)
        {
            int rewardIndex = this.GetRewardIndex(reward);
            return rewardIndex >= this.Config.LevelRushRewards.Count - 1;
        }

        public bool HasUnclaimedRewards
        {
            get
            {
                if (!this.levelRushFeatureHandler.HasActiveFeature())
                {
                    return false;
                }
                int levelIndexFromRewardIndex = this.GetLevelIndexFromRewardIndex(this.LatestRewardClaimed + 1);
                return this.mainProgression.GetFarthestUnlockedLevelIndex() >= levelIndexFromRewardIndex;
            }
        }

        private int LevelCount
        {
            get
            {
                return this.mainLevelsIndices.GetMaxAvailableLevelIndex() + 1;
            }
        }

        private int GetLevelIndexFromReward(LevelRushConfig.Reward reward)
        {
            return this.GetLevelIndexFromRewardIndex(this.GetRewardIndex(reward));
        }

        private bool IsRewardClaimed(LevelRushConfig.Reward reward)
        {
            int latestRewardClaimed = this.LatestRewardClaimed;
            int rewardIndex = this.GetRewardIndex(reward);
            return latestRewardClaimed >= rewardIndex;
        }

        private int LatestRewardClaimed
        {
            get
            {
                if (this.levelRushFeatureHandler.CustomData == null)
                {
                    return -1;
                }
                return this.levelRushFeatureHandler.CustomData.LatestRewardClaimed;
            }
            set
            {
                this.levelRushFeatureHandler.CustomData.LatestRewardClaimed = value;
            }
        }

        private int StartLevel
        {
            get
            {
                if (this.levelRushFeatureHandler.CustomData == null)
                {
                    return -1;
                }
                return this.levelRushFeatureHandler.CustomData.StartLevel;
            }
        }

        private LevelRushConfig Config
        {
            get
            {
                return this.configGetter.Get();
            }
        }

        private readonly ILevelRushFeatureHandler levelRushFeatureHandler;

        private readonly InventoryManager inventoryManager;

        private readonly IMainLevelsIndices mainLevelsIndices;

        private readonly IConfigGetter<LevelRushConfig> configGetter;

        private readonly IMainProgression mainProgression;
    }
}
