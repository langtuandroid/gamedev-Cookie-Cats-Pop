using System;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class StarTournamentSystem : IStarTournamentSystem
	{
		public StarTournamentSystem(StarTournamentManager manager)
		{
			this.Manager = manager;
		}

		public StarTournamentManager Manager { get; private set; }
	}
}
