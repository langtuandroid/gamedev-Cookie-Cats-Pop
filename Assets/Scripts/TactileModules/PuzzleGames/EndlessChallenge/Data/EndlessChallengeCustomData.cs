using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class EndlessChallengeCustomData : FeatureTypeCustomData
	{
		[JsonSerializable("ahr", null)]
		public int AllTimeHighestRow { get; set; }
	}
}
