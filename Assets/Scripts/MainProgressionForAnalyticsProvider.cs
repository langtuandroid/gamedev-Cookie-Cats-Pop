using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;

public class MainProgressionForAnalyticsProvider : IMainProgressionForAnalytics
{
	public MainProgressionForAnalyticsProvider(IMainProgression mainProgression)
	{
		this.mainProgression = mainProgression;
	}

	public int MaxAvailableLevel
	{
		get
		{
			return this.mainProgression.MaxAvailableLevel;
		}
	}

	public int MaxAvailableLevelHumanNumber
	{
		get
		{
			return this.mainProgression.GetMaxAvailableLevelHumanNumber();
		}
	}

	public int GetFarthestCompletedLevelHumanNumber()
	{
		return this.mainProgression.GetFarthestCompletedLevelHumanNumber();
	}

	public int GetFarthestCompletedLevelIndex()
	{
		return this.mainProgression.GetFarthestCompletedLevelIndex();
	}

	public LevelProxy GetFarthestCompletedLevelProxy()
	{
		return this.mainProgression.GetFarthestCompletedLevelProxy();
	}

	public int GetFarthestUnlockedLevelHumanNumber()
	{
		return this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
	}

	public int GetFarthestUnlockedLevelIndex()
	{
		return this.mainProgression.GetFarthestUnlockedLevelIndex();
	}

	public LevelProxy GetFarthestUnlockedLevelProxy()
	{
		return this.mainProgression.GetFarthestUnlockedLevelProxy();
	}

	private readonly IMainProgression mainProgression;
}
