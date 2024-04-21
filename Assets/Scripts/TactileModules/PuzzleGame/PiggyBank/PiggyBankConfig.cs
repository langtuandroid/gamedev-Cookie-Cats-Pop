using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;

namespace TactileModules.PuzzleGame.PiggyBank
{
	[ConfigProvider("PiggyBankConfig")]
	public class PiggyBankConfig
	{
		[Description("Enable/disable feature")]
		[Platform(new PlatformAttribute.Platform[]
		{
			PlatformAttribute.Platform.android,
			PlatformAttribute.Platform.Default
		})]
		[JsonSerializable("Enabled", null)]
		public bool Enabled { get; set; }

		[Description("Level required for Piggy Bank to start")]
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[Description("Coins added per level played")]
		[JsonSerializable("CoinsPerLevel", null)]
		public int CoinsPerLevel { get; set; }

		[Description("Coins added per booster used")]
		[JsonSerializable("CoinsPerBooster", null)]
		public int CoinsPerBooster { get; set; }

		[Description("Coins required in the Piggy Bank for paid opening to become available")]
		[JsonSerializable("CoinsRequiredForPaidOpening", null)]
		public int CoinsRequiredForPaidOpening { get; set; }

		[Description("The coin capacity of the Piggy Bank when the feature starts")]
		[JsonSerializable("InitialCapacity", null)]
		public int InitialCapacity { get; set; }

		[Description("How much the coin capacity should increase per paid opening")]
		[JsonSerializable("CapacityIncrease", null)]
		public int CapacityIncrease { get; set; }

		[Description("Maximum coin capacity to which the capacity of the Piggy Bank can increase through paid openings")]
		[JsonSerializable("MaximumCapacity", null)]
		public int MaximumCapacity { get; set; }

		[Description("First interval between when the Piggy Bank becomes available and the level at which the first free opening occurs")]
		[JsonSerializable("InitialInterval", null)]
		public int InitialInterval { get; set; }

		[Description("Subsequent intervals between Piggy Bank free openings")]
		[JsonSerializable("Interval", null)]
		public int Interval { get; set; }

		[Description("Items that should be included in the Piggy Bank Offer (shown after free opening)")]
		[JsonSerializable("OfferItems", typeof(ItemAmount))]
		public List<ItemAmount> OfferItems { get; set; }
	}
}
