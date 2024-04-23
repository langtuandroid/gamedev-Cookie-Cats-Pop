using System;

namespace TactileModules.PuzzleGame.ScheduledBooster.Analytics
{
	public static class ScheduledBoosterAnalytics
	{
		public static void LogLimitedBoosterUsed(string boosterType, bool isFree, int timeLeft)
		{
			TactileAnalytics.Instance.LogEvent(new ScheduledBoosterAnalytics.LimitedBoosterUsed(boosterType, isFree, timeLeft), -1.0, null);
		}

		[TactileAnalytics.EventAttribute("limitedBoosterUsed", true)]
		private class LimitedBoosterUsed : BasicEvent
		{
			public LimitedBoosterUsed(string boosterType, bool isFree, int timeLeft)
			{
				this.BoosterType = boosterType;
				this.IsFree = isFree;
				this.TimeLeft = timeLeft;
			}

			private TactileAnalytics.RequiredParam<string> BoosterType { get; set; }

			private TactileAnalytics.RequiredParam<bool> IsFree { get; set; }

			private TactileAnalytics.RequiredParam<int> TimeLeft { get; set; }
		}
	}
}
