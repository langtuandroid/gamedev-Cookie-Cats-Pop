using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	[RequireAll]
	public class HotStreakTier
	{
		public HotStreakTier()
		{
			this.Bonuses = new List<ItemAmount>();
		}

		[Description("How many level wins are required to reach this tier")]
		[JsonSerializable("RequiredWins", null)]
		public int RequiredWins { get; set; }

		[Description("Bonuses awarded at each level start for being at this tier")]
		[JsonSerializable("Bonuses", typeof(ItemAmount))]
		public List<ItemAmount> Bonuses { get; set; }
	}
}
