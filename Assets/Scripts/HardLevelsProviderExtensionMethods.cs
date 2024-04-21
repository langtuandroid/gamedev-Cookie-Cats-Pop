using System;
using Tactile;

public static class HardLevelsProviderExtensionMethods
{
	public static DifficultyConfig Config(this IHardLevelsProvider hardLevelsProvider)
	{
		return ConfigurationManager.Get<DifficultyConfig>();
	}
}
