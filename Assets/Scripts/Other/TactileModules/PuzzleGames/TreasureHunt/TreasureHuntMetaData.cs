using System;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public class TreasureHuntMetaData : FeatureMetaData
	{
		[Description("Select level sequence to use for Treasure Hunt. Leave at 0 for random sequence.")]
		[JsonSerializable("levelSetOverrideIndex", null)]
		public int levelSetOverrideIndex { get; set; }
	}
}
