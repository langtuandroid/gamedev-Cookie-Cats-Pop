using System;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	public static class EndlessChallengeAnalytics
	{
		public static void LogEndlessChallengePlayedEvent(int rowReached, int leaderboardRowBest, int rowBest, int leaderboardRank, int checkpointsReached, int cookieJarSmall, int cookieJarFillUp, int boosterRainbow, int boosterFinalPower, int coins, int extraMoves, int missionPops, int movesTotal, int movesUsed, int levelSecondsPlayed, int pregameBoostersUsed, int ingameBoostersUsed, string levelSessionId)
		{
			EndlessChallengeAnalytics.EndlessChallengePlayedEvent endlessChallengePlayedEvent = new EndlessChallengeAnalytics.EndlessChallengePlayedEvent(rowReached, leaderboardRowBest, rowBest, leaderboardRank, checkpointsReached, cookieJarSmall, cookieJarFillUp, boosterRainbow, boosterFinalPower, coins, extraMoves, missionPops, movesTotal, movesUsed, levelSecondsPlayed, pregameBoostersUsed, ingameBoostersUsed);
			endlessChallengePlayedEvent.SetLevelPlayingParameters(levelSessionId);
			TactileAnalytics.Instance.LogEvent(endlessChallengePlayedEvent, -1.0, null);
		}

		[TactileAnalytics.EventAttribute("endlessChallengePlayed", true)]
		private class EndlessChallengePlayedEvent : BasicEvent
		{
			public EndlessChallengePlayedEvent(int rowReached, int leaderboardRowBest, int rowBest, int leaderboardRank, int checkpointsReached, int cookieJarSmall, int cookieJarFillUp, int boosterRainbow, int boosterFinalPower, int coins, int extraMoves, int missionPops, int movesTotal, int movesUsed, int levelSecondsPlayed, int pregameBoostersUsed, int ingameBoostersUsed)
			{
				this.RowReached = rowReached;
				this.LeaderboardRowBest = leaderboardRowBest;
				this.RowBest = rowBest;
				this.LeaderboardRank = leaderboardRank;
				this.CheckpointsReached = checkpointsReached;
				this.CookieJarSmall = cookieJarSmall;
				this.CookieJarFillUp = cookieJarFillUp;
				this.BoosterRainbow = boosterRainbow;
				this.BoosterFinalPower = boosterFinalPower;
				this.Coins = coins;
				this.ExtraMoves = extraMoves;
				this.MissionPops = missionPops;
				this.MovesTotal = movesTotal;
				this.MovesUsed = movesUsed;
				this.Levelsecondsplayed = levelSecondsPlayed;
				this.PregameBoostersUsed = pregameBoostersUsed;
				this.IngameBoostersUsed = ingameBoostersUsed;
			}

			private TactileAnalytics.RequiredParam<int> RowReached { get; set; }

			private TactileAnalytics.RequiredParam<int> LeaderboardRowBest { get; set; }

			private TactileAnalytics.RequiredParam<int> RowBest { get; set; }

			private TactileAnalytics.RequiredParam<int> LeaderboardRank { get; set; }

			private TactileAnalytics.RequiredParam<int> CheckpointsReached { get; set; }

			private TactileAnalytics.RequiredParam<int> CookieJarSmall { get; set; }

			private TactileAnalytics.RequiredParam<int> CookieJarFillUp { get; set; }

			private TactileAnalytics.RequiredParam<int> BoosterRainbow { get; set; }

			private TactileAnalytics.RequiredParam<int> BoosterFinalPower { get; set; }

			private TactileAnalytics.RequiredParam<int> Coins { get; set; }

			private TactileAnalytics.RequiredParam<int> ExtraMoves { get; set; }

			private TactileAnalytics.RequiredParam<int> MissionPops { get; set; }

			private TactileAnalytics.RequiredParam<int> MovesTotal { get; set; }

			private TactileAnalytics.RequiredParam<int> MovesUsed { get; set; }

			private TactileAnalytics.OptionalParam<int> Levelsecondsplayed { get; set; }

			private TactileAnalytics.OptionalParam<int> PregameBoostersUsed { get; set; }

			private TactileAnalytics.OptionalParam<int> IngameBoostersUsed { get; set; }
		}
	}
}
