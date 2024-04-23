using System;
using Tactile;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public static class TreasureHuntManagerProviderExtensions
	{
		public static TreasureHuntConfig Config(this ITreasureHuntProvider treasureHuntProvider)
		{
			return ConfigurationManager.Get<TreasureHuntConfig>();
		}
	}
}
