using System;

namespace TactileModules.PuzzleGame.HotStreak.Analytics
{
	public static class HotStreakAnalytics
	{
		public static void LogStreakTierReached(string featureInstanceID, int levelNumber, int tierReached)
		{
			TactileAnalytics.Instance.LogEvent(new HotStreakAnalytics.HotStreakTierReached(featureInstanceID, levelNumber, tierReached), -1.0, null);
		}

		public static void LogStreakLost(string featureInstanceID, int levelNumber, int streakLength, int tierReached)
		{
			TactileAnalytics.Instance.LogEvent(new HotStreakAnalytics.HotStreakLost(featureInstanceID, levelNumber, tierReached, streakLength), -1.0, null);
		}

		private class HotStreakBasicEven : BasicEvent
		{
			public HotStreakBasicEven(string featureInstanceId, int levelNumber, int reachedTier)
			{
				this.FeatureInstanceID = featureInstanceId;
				this.LevelMapNumber = levelNumber;
				this.TierReached = reachedTier;
			}

			private TactileAnalytics.RequiredParam<string> FeatureInstanceID { get; set; }

			private TactileAnalytics.RequiredParam<int> LevelMapNumber { get; set; }

			private TactileAnalytics.RequiredParam<int> TierReached { get; set; }
		}

		[TactileAnalytics.EventAttribute("hotStreakTierReached", true)]
		private class HotStreakTierReached : HotStreakAnalytics.HotStreakBasicEven
		{
			public HotStreakTierReached(string featureInstanceId, int levelNumber, int reachedTier) : base(featureInstanceId, levelNumber, reachedTier)
			{
			}
		}

		[TactileAnalytics.EventAttribute("hotStreakLost", true)]
		private class HotStreakLost : HotStreakAnalytics.HotStreakBasicEven
		{
			public HotStreakLost(string featureInstanceId, int levelNumber, int reachedTier, int streakLength) : base(featureInstanceId, levelNumber, reachedTier)
			{
				this.StreakLength = streakLength;
			}

			private TactileAnalytics.RequiredParam<int> StreakLength { get; set; }
		}
	}
}
