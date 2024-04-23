using System;
using System.Collections.Generic;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

[RequireAll]
public class HardLevelsMetaData : FeatureMetaData
{
	[StringEnum(typeof(HardLevelsMetaData), "GetIdentifiers")]
	[Description("The identifier of the taget database")]
	[JsonSerializable("LevelDatabaseContext", null)]
	public string LevelDatabaseContext { get; set; }

	[Description("The range of levels that should be hard")]
	[JsonSerializable("LevelRange", typeof(HardLevelsMetaData.HardLevelRange))]
	public HardLevelsMetaData.HardLevelRange LevelRange { get; set; }

	[Description("Should this set of hard levels show popups when starting, ending and expiring?")]
	[JsonSerializable("ShowInfoViews", null)]
	public bool ShowInfoViews { get; set; }

	private static List<string> GetIdentifiers()
	{
		return new List<string>();
	}

	[RequireAll]
	public class HardLevelRange
	{
		[Description("The first index of the range (inclusive)")]
		[JsonSerializable("FromHumanNumber", null)]
		public int FromHumanNumber { get; set; }

		[Description("The last index of the range (inclusive)")]
		[JsonSerializable("ToHumanNumber", null)]
		public int ToHumanNumber { get; set; }
	}
}
