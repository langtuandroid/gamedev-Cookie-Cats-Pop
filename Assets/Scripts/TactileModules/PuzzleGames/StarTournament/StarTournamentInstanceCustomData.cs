using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StarTournament
{
	[ObsoleteJsonName("shv")]
	public class StarTournamentInstanceCustomData : FeatureInstanceCustomData
	{
		public StarTournamentInstanceCustomData()
		{
			this.StartedViewPresented = false;
			this.JoinedInstanceId = string.Empty;
		}

		[JsonSerializable("svp", null)]
		public bool StartedViewPresented { get; set; }

		[JsonSerializable("jid", null)]
		public string JoinedInstanceId { get; set; }
	}
}
