using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/ExperienceSettings.asset")]
public class ExperienceSettings : SingletonAsset<ExperienceSettings>
{
	public ExperienceSettings.Entry GetActive()
	{
		ExperienceConfig experienceConfig = ConfigurationManager.Get<ExperienceConfig>();
		int farthestUnlockedLevelIndex = MainProgressionManager.Instance.GetFarthestUnlockedLevelIndex();
		if (farthestUnlockedLevelIndex > experienceConfig.HighSpeedAtLevel)
		{
			return this.fast;
		}
		if (farthestUnlockedLevelIndex > experienceConfig.MediumSpeedAtLevel)
		{
			return this.medium;
		}
		return this.slow;
	}

	[SerializeField]
	private ExperienceSettings.Entry slow;

	[SerializeField]
	private ExperienceSettings.Entry medium;

	[SerializeField]
	private ExperienceSettings.Entry fast;

	[Serializable]
	public class Entry
	{
		public float kittenFlyToHudDuration = 1f;
	}
}
