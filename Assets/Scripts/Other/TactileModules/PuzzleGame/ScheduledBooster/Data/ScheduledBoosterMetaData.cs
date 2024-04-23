using System;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	public class ScheduledBoosterMetaData : FeatureMetaData
	{
		[StringEnum(typeof(ScheduledBoosterType), "GetIdentifiers")]
		[JsonSerializable("LimitedAvailabilityBoosterType", null)]
		public string ScheduledBoosterType { get; set; }

		[JsonSerializable("BoosterPrice", null)]
		public int BoosterPrice { get; set; }
	}
}
