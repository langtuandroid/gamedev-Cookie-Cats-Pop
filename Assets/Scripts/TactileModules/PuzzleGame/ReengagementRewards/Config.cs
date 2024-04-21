using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;

namespace TactileModules.PuzzleGame.ReengagementRewards
{
	[ConfigProvider("ReengagementRewardsConfig")]
	public class Config
	{
		[Description("Days required to be away from the game to trigger reengagement rewards")]
		[JsonSerializable("DaysAwayRequired", null)]
		public int DaysAwayRequired { get; set; }

		[IgnoreProperty]
		[Description("Rewards for reengagement. Obsolete")]
		[Obsolete("Use 'Items' property instead. This requires change on configuration scheme")]
		[JsonSerializable("Reward", null)]
		public ItemPackage Reward { get; set; }

		[Description("Rewards for reengagement")]
		[JsonSerializable("Items", typeof(ItemAmount))]
		public List<ItemAmount> Items { get; set; }
	}
}
