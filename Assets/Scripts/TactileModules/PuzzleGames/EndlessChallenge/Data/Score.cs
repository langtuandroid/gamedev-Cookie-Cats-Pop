using System;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	public class Score
	{
		[JsonSerializable("maxRows", null)]
		public int MaxRows { get; set; }
	}
}
