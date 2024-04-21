using System;
using Tactile;

public static class OneLifeChallengeManagerExtensionMethods
{
	public static OneLifeChallengeConfig Config(this IOneLifeChallengeProvider oneLifeChallengeProvider)
	{
		return ConfigurationManager.Get<OneLifeChallengeConfig>();
	}
}
