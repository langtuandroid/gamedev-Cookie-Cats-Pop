using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class MissionEndEvent : BasicMissionEvent
	{
		protected MissionEndEvent(ILevelAttempt levelAttempt) : base(levelAttempt)
		{
			ILevelSessionStats stats = levelAttempt.Stats;
			this.ContinuesUsed = levelAttempt.NumberOfContinuesUsed;
			this.FreebiePaid = stats.FreebiePaid;
			this.FreebieVideoWatched = stats.FreebieVideoWatched;
			this.FreebieType = stats.FreebieType;
			this.IngameBoostersUsed = stats.IngameBoostersUsed;
			this.LevelPoints = stats.Score;
			this.LevelSecondsPlayed = (int)levelAttempt.SecondsPlayed;
			this.LevelGoalPiecesCollected = stats.GoalPiecesCollected;
			this.MovesAddedByContinue = stats.MovesAddedByContinue;
			this.MovesAddedByFreebie = stats.MovesAddedByFreebie;
			this.MovesAddedByGamePiece = stats.MovesAddedByGamePiece;
			this.MovesAddedByInGameBooster = stats.MovesAddedByInGameBooster;
			this.MovesAddedByPreGameBooster = stats.MovesAddedByPreGameBooster;
			this.MovesLeft = stats.MovesLeftBeforeAftermath;
			this.MovesUsed = stats.MovesUsed;
			this.PresentsCollected = stats.PresentsCollected;
		}

		private TactileAnalytics.RequiredParam<bool> FreebiePaid { get; set; }

		private TactileAnalytics.RequiredParam<bool> FreebieVideoWatched { get; set; }

		private TactileAnalytics.OptionalParam<string> FreebieType { get; set; }

		private TactileAnalytics.RequiredParam<int> LevelGoalPiecesCollected { get; set; }

		private TactileAnalytics.RequiredParam<int> ContinuesUsed { get; set; }

		private TactileAnalytics.RequiredParam<int> IngameBoostersUsed { get; set; }

		private TactileAnalytics.RequiredParam<int> LevelPoints { get; set; }

		private TactileAnalytics.RequiredParam<int> LevelSecondsPlayed { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesAddedByContinue { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesAddedByFreebie { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesAddedByGamePiece { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesAddedByInGameBooster { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesAddedByPreGameBooster { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesLeft { get; set; }

		private TactileAnalytics.RequiredParam<int> MovesUsed { get; set; }

		private TactileAnalytics.RequiredParam<int> PresentsCollected { get; set; }
	}
}
