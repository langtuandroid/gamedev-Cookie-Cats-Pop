using System;
using TactileModules.FeatureManager.DataClasses;

public class HardLevelsInstanceCustomData : FeatureInstanceCustomData
{
	[JsonSerializable("ShowedStartedPopup", null)]
	public bool ShowedStartedPopup { get; set; }
}
