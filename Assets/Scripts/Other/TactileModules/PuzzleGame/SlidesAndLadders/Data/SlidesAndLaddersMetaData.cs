using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public class SlidesAndLaddersMetaData : FeatureMetaData
	{
		[JsonSerializable("Rewards", typeof(ItemAmount))]
		public List<ItemAmount> Rewards { get; set; }
	}
}
