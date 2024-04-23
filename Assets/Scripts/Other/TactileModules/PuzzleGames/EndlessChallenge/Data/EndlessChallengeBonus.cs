using System;
using ConfigSchema;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class EndlessChallengeBonus
	{
		[Required]
		[JsonSerializable("Type", null)]
		public EndlessChallengeBonusType Type { get; set; }

		[Required]
		[JsonSerializable("Amount", null)]
		public int Amount { get; set; }
	}
}
