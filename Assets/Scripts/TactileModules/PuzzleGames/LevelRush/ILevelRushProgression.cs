using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface ILevelRushProgression
	{
		bool HasUnclaimedRewards { get; }

		bool IsRewardOnNextLevelToComplete(int rewardIndex);

		List<LevelRushConfig.Reward> GetUnclaimedRewards();

		int GetRewardIndex(LevelRushConfig.Reward reward);

		bool IsRewardBeyondFarthestUnlockedLevel(int rewardIndex);

		int GetLevelIndexFromRewardIndex(int rewardIndex);

		LevelRushConfig.Reward GetReward(int rewardIndex);

		LevelRushConfig.Reward ClaimNextUnclaimedReward();

		void AddRewardToInventory(LevelRushConfig.Reward reward);

		bool IsLastReward(LevelRushConfig.Reward reward);
	}
}
