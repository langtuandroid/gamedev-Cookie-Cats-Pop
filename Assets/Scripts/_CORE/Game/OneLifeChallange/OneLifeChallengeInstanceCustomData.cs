using System;
using TactileModules.FeatureManager.DataClasses;

public class OneLifeChallengeInstanceCustomData : FeatureInstanceCustomData
{
	public OneLifeChallengeInstanceCustomData()
	{
		this.Revision = 0;
		this.FarthestCompletedLevel = -1;
		this.RewardClaimed = false;
	}

	[JsonSerializable("rv", null)]
	public int Revision { get; set; }

	[JsonSerializable("pr", null)]
	public int FarthestCompletedLevel { get; set; }

	[JsonSerializable("c", null)]
	public bool RewardClaimed { get; set; }
}
