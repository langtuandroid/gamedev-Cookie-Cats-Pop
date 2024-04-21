using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public class SlidesAndLaddersRewards : ISlidesAndLaddersRewards
	{
		public SlidesAndLaddersRewards(IDataProvider<SlidesAndLaddersInstanceCustomData> customData, IDataProvider<SlidesAndLaddersMetaData> metaData, IDataProvider<SlidesAndLaddersConfig> config, ISlidesAndLaddersLevelDatabase levelDatabase)
		{
			this.customData = customData;
			this.metaData = metaData;
			this.config = config;
			this.levelDatabase = levelDatabase;
		}

		public List<ItemAmount> AddedChestRewards
		{
			get
			{
				return this.customData.Get().AddedChestRewards;
			}
		}

		public List<ItemAmount> GetFeatureRewards()
		{
			List<ItemAmount> list = new List<ItemAmount>();
			list.AddRange(this.AddedChestRewards);
			list.AddRange(this.metaData.Get().Rewards);
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

		public bool HasClaimedLevelRewardAtIndex(int index)
		{
			return this.customData.Get().RewardsClaimed.Contains(index);
		}

		public void ClaimLevelRewardAtIndex(int index)
		{
			this.customData.Get().RewardsClaimed.Add(index);
		}

		public void AddChestRewards(List<ItemAmount> rewards)
		{
			for (int i = rewards.Count - 1; i >= 0; i--)
			{
				if (rewards[i].Amount <= 0)
				{
					rewards.RemoveAt(i);
				}
			}
			this.customData.Get().AddedChestRewards.AddRange(rewards);
		}

		public List<ItemAmount> GetRandomSlidesRewards(int giveNumberOfRewards)
		{
			List<ItemAmount> list = new List<ItemAmount>();
			for (int i = 0; i < giveNumberOfRewards; i++)
			{
				list.Add(this.config.Get().SlidesRewards[UnityEngine.Random.Range(0, this.config.Get().SlidesRewards.Count)]);
			}
			return list;
		}

		public bool LevelHasValidChest(int levelIndex)
		{
			return !this.customData.Get().RewardsClaimed.Contains(levelIndex) && this.levelDatabase.IsTreasureLevel(levelIndex);
		}

		public int GetChestRank(int index)
		{
			return this.levelDatabase.GetChestRank(index);
		}

		private readonly IDataProvider<SlidesAndLaddersInstanceCustomData> customData;

		private readonly IDataProvider<SlidesAndLaddersMetaData> metaData;

		private readonly IDataProvider<SlidesAndLaddersConfig> config;

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;
	}
}
