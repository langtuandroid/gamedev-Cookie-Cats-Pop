using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelDash
{
	public class LevelDashTypeCustomData : FeatureTypeCustomData
	{
		public LevelDashTypeCustomData()
		{
			this.MyRankForPreviousLevelDash = 0;
		}

		[JsonSerializable("mrpld", null)]
		public int MyRankForPreviousLevelDash { get; set; }
	}
}
