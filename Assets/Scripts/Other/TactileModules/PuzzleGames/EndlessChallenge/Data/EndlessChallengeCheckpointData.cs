using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	[RequireAll]
	public class EndlessChallengeCheckpointData
	{
		public EndlessChallengeCheckpointData()
		{
			this.EndlessChallengeBonuses = new List<EndlessChallengeBonus>();
			this.InventoryBonuses = new List<ItemAmount>();
		}

		[Description("Where to position the checkpoint, will loop after the last entry")]
		[JsonSerializable("PositionAtRow", null)]
		public int PositionAtRow { get; set; }

		[Description("Bonuses used instantly in endless challenge")]
		[JsonSerializable("EndlessChallengeBonuses", typeof(EndlessChallengeBonus))]
		public List<EndlessChallengeBonus> EndlessChallengeBonuses { get; set; }

		[Description("Bonuses for the inventory")]
		[JsonSerializable("InventoryBonuses", typeof(ItemAmount))]
		public List<ItemAmount> InventoryBonuses { get; set; }
	}
}
