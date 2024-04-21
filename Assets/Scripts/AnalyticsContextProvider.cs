using System;
using TactileModules.PuzzleGames.GameCore.Analytics;

public class AnalyticsContextProvider : IAnalyticsContextProvider
{
	public string CreateContextString()
	{
		return Analytics.Instance.masterContext.value;
	}
}
