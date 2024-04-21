using System;
using Tactile;

namespace TactileModules.PuzzleGames.LevelDash.Providers
{
	public static class LevelDashDataProviderExtensionMethods
	{
		public static LevelDashConfig Config(this ILevelDashDataProvider provider)
		{
			return ConfigurationManager.Get<LevelDashConfig>();
		}
	}
}
