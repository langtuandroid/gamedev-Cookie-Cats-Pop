using System;
using Tactile;

namespace TactileModules.PuzzleGames.StarTournament
{
	public static class StarTournamentProviderExtensionMethods
	{
		public static StarTournamentConfig Config(this IStarTournamentProvider provider)
		{
			return ConfigurationManager.Get<StarTournamentConfig>();
		}
	}
}
