using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	[SingletonAssetPath("Assets/[Database]/Resources/EndlessChallengeSetup.asset")]
	public class EndlessChallengeSetup : SingletonAsset<EndlessChallengeSetup>
	{
		public EndlessCheckpoint endlessCheckpoint;

		public List<LevelVisuals.FreebieEntry> endlessChallengeFreebeeBoosters;

		public List<EndlessChallengeBonusInfo> endlessChallengeBonusInfos;
	}
}
