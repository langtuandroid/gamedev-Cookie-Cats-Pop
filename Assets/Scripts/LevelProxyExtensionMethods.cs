using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.MainLevels;

public static class LevelProxyExtensionMethods
{
	private static HardLevelsManager HardLevelsManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<HardLevelsManager>();
		}
	}

	public static string GetContextWithFeatures(this LevelProxy levelProxy)
	{
		List<string> list = new List<string>();
		if (levelProxy.RootDatabase is MainLevelDatabase)
		{
			list.AddRange(levelProxy.AnalyticsDescriptors);
			list.AddRange(FeatureManager.Instance.GetAllMainMapActiveFeatureTypes());
		}
		else
		{
			list.Add(levelProxy.AnalyticsDescriptors[0]);
		}
		bool flag = list.Contains(LevelProxyExtensionMethods.HardLevelsManager.FeatureType) && !LevelProxyExtensionMethods.HardLevelsManager.IsLevelHard(levelProxy);
		if (flag)
		{
			list.Remove(LevelProxyExtensionMethods.HardLevelsManager.FeatureType);
		}
		return string.Join(",", list.ToArray());
	}

	public static int GetHardStars(this LevelProxy level)
	{
		return MainProgressionManager.Instance.GetHardStars(level);
	}
}
