using System;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	public static class HotStreakLocalizationData
	{
		public static string LevelAbandonTitleText()
		{
			return L.Get("Are you sure?");
		}

		public static string LevelAbandonDescriptionText()
		{
			return L.Get("You’ll lose your Hot Streak bonuses if you leave the level now.");
		}
	}
}
