using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	public class ScheduledBoosterInstanceCustomData : FeatureInstanceCustomData
	{
		public ScheduledBoosterInstanceCustomData()
		{
			this.NumberOfBoostersUsed = 0;
		}

		[JsonSerializable("nbu", null)]
		public int NumberOfBoostersUsed { get; set; }
	}
}
