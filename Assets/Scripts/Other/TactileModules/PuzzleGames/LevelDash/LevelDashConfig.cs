using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelDash
{
	[ConfigProvider("LevelDashConfig")]
	public class LevelDashConfig
	{
		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[JsonSerializable("MinimumRequiredLevels", null)]
		public int MinimumRequiredLevels { get; set; }

		[JsonSerializable("Rewards", typeof(LevelDashConfig.Reward))]
		public List<LevelDashConfig.Reward> Rewards { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }

		[ObsoleteJsonName("Package")]
		public class Reward : IEquatable<LevelDashConfig.Reward>
		{
			public Reward()
			{
				this.Rank = -1;
				this.Items = new List<ItemAmount>();
			}

			[JsonSerializable("Rank", null)]
			public int Rank { get; set; }

			[JsonSerializable("Items", typeof(ItemAmount))]
			public List<ItemAmount> Items { get; set; }

			public bool Equals(LevelDashConfig.Reward other)
			{
				return other != null && this.Rank == other.Rank && (this.Items != null || other.Items == null) && (this.Items == null || other.Items != null) && (this.Items == null || other.Items == null || this.EqualItemsContent(this.Items, other.Items));
			}

			private bool EqualItemsContent(List<ItemAmount> localItems, List<ItemAmount> cloudItems)
			{
				List<ItemAmount> list = (localItems.Count >= cloudItems.Count) ? localItems : cloudItems;
				List<ItemAmount> list2 = (localItems.Count >= cloudItems.Count) ? cloudItems : localItems;
				for (int i = 0; i < list2.Count; i++)
				{
					if (!list.Contains(list2[i]))
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
