using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public interface ISlidesAndLaddersRewards
	{
		List<ItemAmount> AddedChestRewards { get; }

		List<ItemAmount> GetFeatureRewards();

		bool HasClaimedLevelRewardAtIndex(int index);

		void ClaimLevelRewardAtIndex(int index);

		void AddChestRewards(List<ItemAmount> rewards);

		List<ItemAmount> GetRandomSlidesRewards(int rewards);

		bool LevelHasValidChest(int index);

		int GetChestRank(int index);
	}
}
