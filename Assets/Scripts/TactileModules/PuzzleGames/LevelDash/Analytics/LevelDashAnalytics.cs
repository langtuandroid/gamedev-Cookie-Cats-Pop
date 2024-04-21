using System;

namespace TactileModules.PuzzleGames.LevelDash.Analytics
{
	public static class LevelDashAnalytics
	{
		public static void LogLevelDashStarted(int startingLevel, string featureInstanceId, int bucketId)
		{
			LevelDashAnalytics.LevelDashStarted eventObject = new LevelDashAnalytics.LevelDashStarted(startingLevel, featureInstanceId, bucketId);
			TactileAnalytics.Instance.LogEvent(eventObject, -1.0, null);
		}

		public static void LogLevelDashEnded(int startingLevel, int endingLevel, string featureInstanceId, int bucketId)
		{
			LevelDashAnalytics.LevelDashEnded eventObject = new LevelDashAnalytics.LevelDashEnded(startingLevel, endingLevel, featureInstanceId, bucketId);
			TactileAnalytics.Instance.LogEvent(eventObject, -1.0, null);
		}

		private class BaseLevelDashEvent : BasicEvent
		{
			public BaseLevelDashEvent(int startingLvl, string subContextId, int bucketIdentifier)
			{
				this.startingLevel = startingLvl;
				this.featureInstanceId = subContextId;
				this.bucketId = bucketIdentifier;
			}

			private TactileAnalytics.RequiredParam<int> startingLevel { get; set; }

			private TactileAnalytics.RequiredParam<string> featureInstanceId { get; set; }

			private TactileAnalytics.RequiredParam<int> bucketId { get; set; }
		}

		[TactileAnalytics.EventAttribute("levelDashStarted", true)]
		private class LevelDashStarted : LevelDashAnalytics.BaseLevelDashEvent
		{
			public LevelDashStarted(int startingLvl, string subContextId, int bucketIdentifier) : base(startingLvl, subContextId, bucketIdentifier)
			{
			}
		}

		[TactileAnalytics.EventAttribute("levelDashEnded", true)]
		private class LevelDashEnded : LevelDashAnalytics.BaseLevelDashEvent
		{
			public LevelDashEnded(int startingLvl, int endingLvl, string subContextId, int bucketIdentifier) : base(startingLvl, subContextId, bucketIdentifier)
			{
				this.endingLevel = endingLvl;
			}

			private TactileAnalytics.RequiredParam<int> endingLevel { get; set; }
		}
	}
}
