using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StarTournament
{
	public class StarTournamentTypeCustomData : FeatureTypeCustomData
	{
		public StarTournamentTypeCustomData()
		{
			this.OldTournamentInfo = new OldStartTournamentInfo();
		}

		[JsonSerializable("ost", null)]
		public OldStartTournamentInfo OldTournamentInfo { get; set; }
	}
}
