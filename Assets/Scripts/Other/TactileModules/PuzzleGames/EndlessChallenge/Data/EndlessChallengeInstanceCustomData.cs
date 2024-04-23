using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class EndlessChallengeInstanceCustomData : FeatureInstanceCustomData
	{
		public EndlessChallengeInstanceCustomData()
		{
			this.HighestRow = 0;
		}

		[JsonSerializable("echr", null)]
		public int HighestRow { get; set; }
	}
}
