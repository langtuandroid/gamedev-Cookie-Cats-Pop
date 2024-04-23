using System;
using System.Collections.Generic;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class EndlessChallengeMetaData : FeatureMetaData
	{
		public EndlessChallengeMetaData()
		{
			this.EndlessChallengeCheckpoints = new List<EndlessChallengeCheckpointData>();
		}

		[Required]
		[Description("Endless Challenge checkpoint intervals")]
		[JsonSerializable("EndlessChallengeCheckpoints", typeof(EndlessChallengeCheckpointData))]
		public List<EndlessChallengeCheckpointData> EndlessChallengeCheckpoints { get; set; }
	}
}
