using System;
using TactileModules.PuzzleGames.EndlessChallenge.Data;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	[Serializable]
	public class EndlessChallengeBonusInfo
	{
		public EndlessChallengeBonusType type;

		public BoosterLogic logicPrefab;

		[UISpriteName]
		public string checkpointSprite;
	}
}
