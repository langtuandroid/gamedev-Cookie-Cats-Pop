using System;

public class BasicEvent
{
	private TactileAnalytics.OptionalParam<string> AllActiveFeatureTypes { get; set; }

	private TactileAnalytics.OptionalParam<string> AllActiveFeatureInstances { get; set; }

	public void SetFeatureManagerParameters(string allActiveFeatureTypes, string allActiveFeatureInstanceIds)
	{
		this.AllActiveFeatureTypes = allActiveFeatureTypes;
		this.AllActiveFeatureInstances = allActiveFeatureInstanceIds;
	}

	private TactileAnalytics.RequiredParam<int> UserLives { get; set; }

	public void SetInventoryManagerParameters(int userLives)
	{
		this.UserLives = userLives;
	}

	private TactileAnalytics.OptionalParam<string> LevelSessionId { get; set; }

	public void SetLevelPlayingParameters(string levelSessionId)
	{
		this.LevelSessionId = levelSessionId;
	}

	private TactileAnalytics.RequiredParam<int> ConfigVersion { get; set; }

	private TactileAnalytics.OptionalParam<string> FlowLocation { get; set; }

	private TactileAnalytics.OptionalParam<string> Context { get; set; }

	public void SetPuzzleCoreCommonProperties(string flowLocation, int configVersion, string context)
	{
		this.ConfigVersion = configVersion;
		this.FlowLocation = flowLocation;
		this.Context = context;
	}

	private TactileAnalytics.OptionalParam<int> UserProgression { get; set; }

	private TactileAnalytics.OptionalParam<double> UserProgressionWithGates { get; set; }

	public void SetUserProgression(int userProgression, double progressionWithGates)
	{
		this.UserProgression = userProgression;
		this.UserProgressionWithGates = progressionWithGates;
	}
}
